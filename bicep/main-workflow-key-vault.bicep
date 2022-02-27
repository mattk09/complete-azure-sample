@description('Base name of all resourcs (invalid characters will be stripped when required).')
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

var devAccessPolicy = {
  tenantId: subscription().tenantId
  objectId: developerObjectIdKeyVaultAccessPolicy
  permissions: {
    secrets: [
      'get'
      'set'
      'list'
      'delete'
    ]
  }
}

module keyVault 'modules/key-vault.bicep' = {
  name: name
  params: {
    name: name
    location: location
    additionalSecrets: additionalSecrets
    additionalAccessPolicies: skip([
      devAccessPolicy
    ], empty(developerObjectIdKeyVaultAccessPolicy) ? 1 : 0)
  }
}
