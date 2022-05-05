@description('Base name for the functions service.')
param name string = resourceGroup().name

@description('Location for the functions service.')
param location string = resourceGroup().location

@description('AppServicePlan name for the functions service to run under.')
param appServicePlanName string

@description('Name of the Key Vault to use for configuration.')
param keyVaultNameForConfiguration string

@description('Name of Storage Acount.')
param storageAccountName string

@description('Name of Application Insights.')
param applicationInsightsName string

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' existing = {
  name: storageAccountName
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value}'

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' existing = {
  name: appServicePlanName
}

resource functionsApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: appServicePlan.id
    reserved: true
    siteConfig: {
      healthCheckPath: 'api/healthcheck'
      appSettings: [
        {
          // I would prefer this comes from KeyVault, but the functions runtime consumes this before KV can be applied right now
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccountConnectionString
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'KeyVaultNameFromDeployment'
          value: keyVaultNameForConfiguration
        }
        {
          name: 'AzureWebJobsSecretStorageType'
          value: 'keyvault'
        }
        {
          name: 'AzureWebJobsSecretStorageKeyVaultUri'
          value: 'https://${keyVaultNameForConfiguration}${environment().suffixes.keyvaultDns}/'
        }
      ]
    }
  }
}

output functionsAppName string = functionsApp.name
output functionsAppDefaultHostName string = functionsApp.properties.defaultHostName
output functionsAppPrincipalId string = functionsApp.identity.principalId

