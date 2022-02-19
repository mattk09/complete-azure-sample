param name string = resourceGroup().name
param location string = resourceGroup().location

@description('Additional secrets to inject into the keyVault')
@secure()
param additionalSecrets object = {
  items: [
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

var appServicePlanName = name
var appServicePlanSku = {
  name: 'B1'
}
var webAppName = name
var storageAccountName = toLower(take(replace(replace(name, '-', ''), '_', ''), 24))
var appInsightsName = name
var functionsAppName = '${name}-functions'
var keyVaultName = toLower(take(replace(replace(name, '-', ''), '_', ''), 24))
var keyVaultSku = {
  family: 'A'
  name: 'standard'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  kind: 'StorageV2'
  location: location
  sku: {
    name: storageAccountSku
  }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccount.listKeys().keys[0].value}'

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  sku: appServicePlanSku
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|3.1'
    }
    httpsOnly: true
  }
}

resource webAppName_appsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  parent: webApp
  name: 'appsettings'
  properties: {
    KeyVaultNameFromDeployment: keyVaultName
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-10-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    accessPolicies: [
      {
        tenantId: webApp.identity.tenantId
        objectId: webApp.identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
    sku: keyVaultSku
  }
}

resource keyVaultName_ApplicationInsights_InstrumentationKey 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = {
  parent: keyVault
  name: 'ApplicationInsights--InstrumentationKey'
  properties: {
    value: appInsights.properties.InstrumentationKey
    attributes: {
      enabled: true
    }
  }
}

resource keyVaultName_AzureStorageSettings_ConnectionString 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = {
  parent: keyVault
  name: 'AzureStorageSettings--ConnectionString'
  properties: {
    value: storageAccountConnectionString
    attributes: {
      enabled: true
    }
  }
}

resource keyVaultName_additionalSecrets_items_name 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = [for i in range(0, length(additionalSecrets.items)): {
  name: '${keyVaultName}/${additionalSecrets.items[i].name}'
  properties: {
    value: additionalSecrets.items[i].secret
  }
}]

resource functionsApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionsAppName
  location: location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: appServicePlan.id
    reserved: true
    siteConfig: {
      appSettings: [
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
          value: '~3'
        }
      ]
    }
  }
}

output result object = {
  storageEndpoint: storageAccount.properties.primaryEndpoints
  webAppName: webAppName
  functionsAppName: functionsAppName
}