# Deploy to Azure

https://docs.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=userlevel

```bash
az login --use-device-code

az ad sp create-for-rbac \
    --name "github-actions" \
    --role contributor \
    --scopes /subscriptions/1920eb56-847c-4683-8764-7719e29a4828 \
    --sdk-auth

# Show exisiting
az ad sp show --id ff327da3-3916-4376-be29-05494e632fb9
# or all
az ad sp list

```

```json
[
  {
    "cloudName": "AzureCloud",
    "homeTenantId": "a0e8a98b-89b5-45bd-9caf-dcb8a154fb2b",
    "id": "1920eb56-847c-4683-8764-7719e29a4828",
    "isDefault": true,
    "managedByTenants": [],
    "name": "Visual Studio Enterprise Subscription",
    "state": "Enabled",
    "tenantId": "a0e8a98b-89b5-45bd-9caf-dcb8a154fb2b",
    "user": {
      "name": "mattk09@gmail.com",
      "type": "user"
    }
  }
]

# I had to add 'subscriptionId' manually
{
  "clientId": "ff327da3-3916-4376-be29-05494e632fb9",
  "clientSecret": "piPeWCnXMhB-R3emuk3D0yyil7qBJ.dde9",
  "subscriptionId": "1920eb56-847c-4683-8764-7719e29a4828",
  "tenantId": "a0e8a98b-89b5-45bd-9caf-dcb8a154fb2b",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```