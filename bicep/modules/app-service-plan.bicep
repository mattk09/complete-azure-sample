@description('Base name for the app service.')
param name string = resourceGroup().name

@description('Location for the app service.')
param location string = resourceGroup().location

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: name
  location: location
  sku: {
    name: 'B1'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

output appServicePlanName string = appServicePlan.name
output appServicePlanId string = appServicePlan.id
