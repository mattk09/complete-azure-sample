param name string = resourceGroup().name
param location string = resourceGroup().location

@description('Additional secrets to inject into the keyVault')
@secure()
param additionalSecrets object = {
  items: [
    {
      name: 'example-secret-name'
      secret: 'example-secret-value'
    }
  ]
}

var appServicePlanName_var = name
var appServicePlanSku = {
  name: 'S3'
  tier: 'Standard'
  size: 'S3'
  family: 'S'
  capacity: 1
}
var webAppName_var = name
var storageAccountName_var = toLower(take(replace(replace(name, '-', ''), '_', ''), 24))
var storageAccountSku = 'Standard_LRS'
var storageAccountApiVersion = '2018-07-01'
var appInsightsName_var = name
var functionsAppName_var = '${name}-functions'
var keyVaultName_var = toLower(take(replace(replace(name, '-', ''), '_', ''), 24))
var keyVaultSku = {
  family: 'A'
  name: 'Standard'
}

resource appServicePlanName 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: appServicePlanName_var
  location: location
  sku: appServicePlanSku
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webAppName 'Microsoft.Web/sites@2018-11-01' = {
  name: webAppName_var
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app'
  properties: {
    serverFarmId: appServicePlanName.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|3.1'
    }
    httpsOnly: true
  }
}

resource webAppName_appsettings 'Microsoft.Web/sites/config@2018-11-01' = {
  parent: webAppName
  name: 'appsettings'
  properties: {
    KeyVaultNameFromDeployment: keyVaultName_var
  }
}

resource storageAccountName 'Microsoft.Storage/storageAccounts@[variables(\'storageAccountApiVersion\')]' = {
  name: storageAccountName_var
  kind: 'StorageV2'
  location: location
  sku: {
    name: storageAccountSku
  }
}

resource functionsAppName 'Microsoft.Web/sites@2015-08-01' = {
  name: functionsAppName_var
  location: resourceGroup().location
  kind: 'functionapp,linux'
  properties: {
    serverFarmId: appServicePlanName.id
    reserved: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName_var};AccountKey=${listKeys(storageAccountName.id, storageAccountApiVersion).keys[0].value}'
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
  dependsOn: [
    appInsightsName
  ]
}

resource appInsightsName 'Microsoft.Insights/components@2014-04-01' = {
  name: appInsightsName_var
  location: location
  properties: {}
}

resource keyVaultName 'Microsoft.KeyVault/vaults@2018-02-14' = {
  name: keyVaultName_var
  location: location
  properties: {
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    accessPolicies: [
      {
        tenantId: reference('Microsoft.Web/sites/${webAppName_var}', '2018-02-01', 'Full').identity.tenantId
        objectId: reference('Microsoft.Web/sites/${webAppName_var}', '2018-02-01', 'Full').identity.principalId
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
  dependsOn: [
    webAppName
  ]
}

resource keyVaultName_ApplicationInsights_InstrumentationKey 'Microsoft.KeyVault/vaults/secrets@2016-10-01' = {
  parent: keyVaultName
  name: 'ApplicationInsights--InstrumentationKey'
  location: location
  properties: {
    value: appInsightsName.properties.InstrumentationKey
    attributes: {
      enabled: true
    }
  }
}

resource keyVaultName_AzureStorageSettings_ConnectionString 'Microsoft.KeyVault/vaults/secrets@2016-10-01' = {
  parent: keyVaultName
  name: 'AzureStorageSettings--ConnectionString'
  location: location
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName_var};AccountKey=${listKeys(storageAccountName.id, storageAccountApiVersion).keys[0].value}'
    attributes: {
      enabled: true
    }
  }
}

resource keyVaultName_additionalSecrets_items_name 'Microsoft.KeyVault/vaults/secrets@2016-10-01' = [for i in range(0, length(additionalSecrets.items)): {
  name: '${keyVaultName_var}/${additionalSecrets.items[i].name}'
  properties: {
    value: additionalSecrets.items[i].secret
  }
  dependsOn: [
    keyVaultName
  ]
}]

output result object = {
  WebAppName: webAppName_var
  FunctionsAppName: functionsAppName_var
}