// This is pre santized in the parent
@description('Base name for Application Insights.')
param name string = resourceGroup().name

@description('Location for theApplication Insights.')
param location string = resourceGroup().location

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}
