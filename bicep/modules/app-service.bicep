@description('Base name for the web app.')
param name string = resourceGroup().name

@description('Location for the storage account.')
param location string = resourceGroup().location

@description('AppServicePlan name for the web app to run under.')
param appServicePlanName string

@description('Name of the Key Vault use for configuration.')
param keyVaultNameForConfiguration string

@description('Name of functions api.')
param functionsAppHostName string

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' existing = {
  name: appServicePlanName
}

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|6.0'
      healthCheckPath: 'healthcheck'
    }
    httpsOnly: true
  }
}

resource webAppAppsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  parent: webApp
  name: 'appsettings'
  properties: {
    KeyVaultNameFromDeployment: keyVaultNameForConfiguration
    FunctionsAppHostName : functionsAppHostName
  }
}

output webAppName string = webApp.name
output webAppDefaultHostName string = webApp.properties.defaultHostName
output webAppPrincipalId string = webApp.identity.principalId
