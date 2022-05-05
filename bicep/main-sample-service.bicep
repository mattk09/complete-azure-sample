@description('Base name of all resources (invalid characters will be stripped when required).')
param name string = resourceGroup().name

@description('Optional objectId to grant an identity access to the key vault.')
param developerObjectIdKeyVaultAccessPolicy string = ''

@description('Location of all resources.')
param location string = resourceGroup().location

@description('Additional secrets to inject into the key vault.')
@secure()
param additionalSecrets object = {
  secrets: [
    {
      name: 'example-secret-guid'
      secret: newGuid()
    }
  ]
}

@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GZRS'
  'Standard_RAGZRS'
])
param storageAccountSku string = 'Standard_LRS'

var storageAccountName = toLower(take(replace(replace(name, '-', ''), '_', ''), 24))
var functionsAppName = '${name}-functions'
var keyVaultName = toLower(take(replace(name, '_', ''), 24))

module appServicePlan 'modules/app-service-plan.bicep' = {
  name: 'appServicePlan'
  params: {
    name: name
    location: location
  }
}

module storageAccount 'modules/storage-account.bicep' = {
  name: 'storageAccount'
  params: {
    name: storageAccountName
    location: location
    storageAccountSku: storageAccountSku
    usePrivateEndpoint: false
  }
}

module applicationInsights 'modules/application-insights.bicep' = {
  name: 'applicationInsights'
  params: {
    name: name
    location: location
  }
}

resource applicationInsightsExisting 'Microsoft.Insights/components@2020-02-02' existing = {
  name: name
}

var devAccessPolicy = {
  objectId: developerObjectIdKeyVaultAccessPolicy
  principalType: 'User'
  canWrite: true
}

var webAppAccessPolicy = {
  objectId: webApp.outputs.webAppPrincipalId
  principalType: 'ServicePrincipal'
  canWrite: false
}

var functionsAccessPolicy = {
  objectId: functionsApp.outputs.functionsAppPrincipalId
  principalType: 'ServicePrincipal'
  canWrite: true
}

resource storageAccountExisting 'Microsoft.Storage/storageAccounts@2021-08-01' existing = {
  name: storageAccountName
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccountExisting.listKeys().keys[0].value}'

module keyVault 'modules/key-vault.bicep' = {
  name: 'keyVault'
  params: {
    name: keyVaultName
    location: location
    additionalSecrets: {
      secrets: concat([
        {
          name: 'AzureStorageSettings--ConnectionString'
          secret: storageAccountConnectionString
        }
        {
          name: 'ApplicationInsights--InstrumentationKey'
          secret: applicationInsightsExisting.properties.InstrumentationKey
        }
      ], additionalSecrets.secrets)
    }
    additionalAccessPolicies: skip([
      devAccessPolicy
      webAppAccessPolicy
      functionsAccessPolicy
    ], empty(developerObjectIdKeyVaultAccessPolicy) ? 1 : 0)
  }
}

module functionsApp 'modules/functions.bicep' = {
  name: 'functionsService'
  params: {
    name: functionsAppName
    location: location
    appServicePlanName: appServicePlan.outputs.appServicePlanName
    keyVaultNameForConfiguration: keyVaultName
    storageAccountName: storageAccountExisting.name
    applicationInsightsName: applicationInsightsExisting.name
  }
}

module webApp 'modules/app-service.bicep' = {
  name: 'appService'
  params: {
    location: location
    appServicePlanName: appServicePlan.outputs.appServicePlanName
    keyVaultNameForConfiguration: keyVaultName
    functionsAppHostName: functionsApp.outputs.functionsAppDefaultHostName
  }
}

output storageEndpoint object = storageAccount.outputs.storageEndpoint
output webAppName string = webApp.outputs.webAppName
output webAppEndpoint string = 'https://${webApp.outputs.webAppDefaultHostName}/'
output webAppHealthCheckEndpoint string = 'https://${webApp.outputs.webAppDefaultHostName}/healthcheck'
output functionsAppName string = functionsApp.outputs.functionsAppName
output functionsAppHealthCheckEndpoint string = 'https://${functionsApp.outputs.functionsAppDefaultHostName}/api/healthcheck'
