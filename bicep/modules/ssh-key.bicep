@description('Name of the ssh key.')
param name string = resourceGroup().name

@description('Location of the ssh key.')
param location string = resourceGroup().location

resource sshKey 'Microsoft.Compute/sshPublicKeys@2021-11-01' = {
  name: '${name}-sshKey'
  location: location
}

resource generateScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  location: location
  name: 'generateSSHKey'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.32.0'
    retentionInterval: 'PT1H'
    scriptContent: '''
#!/bin/bash
set -euo pipefail

#EXISTING_KEY="$(az sshkey show -g $RESOURCE_GROUP -n $SSHKEY_NAME --query publicKey -o tsv)"
#if [ -z "${EXISTING_KEY}" ]; then
#  echo "creating SSH private key and storing in Key Vault..."
#  PRIVATE_KEY=$(az rest -m post -u "/subscriptions/{subscriptionId}/resourceGroups/${RESOURCE_GROUP}/providers/Microsoft.Compute/sshPublicKeys/${SSHKEY_NAME}/generateKeyPair?api-version=2021-11-01" --query privateKey -o tsv)
#  az keyvault secret set --vault-name $VAULT_NAME --name "${SSHKEY_NAME}-sshPrivateKey" --value "${PRIVATE_KEY}" > /dev/null
#else
#  echo "private key already exists on sshPublicKey resource, skipping..."
#fi

#az sshkey show -g $RESOURCE_GROUP -n $SSHKEY_NAME --query '{publicKey:publicKey}' > $AZ_SCRIPTS_OUTPUT_PATH
# Important variables: https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deployment-script-template#develop-deployment-scripts
echo '{"publicKey": "NA"}' > $AZ_SCRIPTS_OUTPUT_PATH
'''
  }
}

output sshPublicKey string = generateScript.properties.outputs.publicKey
