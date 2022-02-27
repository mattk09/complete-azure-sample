@description('Name of the key vault (must be valid).')
param name string = resourceGroup().name

@description('Location of the key vault.')
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

@description('')
param additionalAccessPolicies array = [
  {
    name: 'example-secret-guid'
    secret: newGuid()
  }
]

var keyVaultName = toLower(take(replace(name, '_', ''), 24))
var keyVaultSku = {
  family: 'A'
  name: 'standard'
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-10-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    enabledForDeployment: false
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    enableSoftDelete: false
    accessPolicies: additionalAccessPolicies
    sku: keyVaultSku
  }
}

resource keyVault_additionalSecrets_items 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = [for i in range(0, length(additionalSecrets.secrets)): {
  name: '${keyVault.name}/${additionalSecrets.secrets[i].name}'
  properties: {
    value: additionalSecrets.secrets[i].secret
  }
}]

output keyVaultName string = keyVault.name
