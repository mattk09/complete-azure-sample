# Introduction

This project is meant to be a general starting point for most common dotnet projects.  It provides boiler plate code for basic core engineering fundamentals, like observability, testing, security, and CI/CD.

Depending on your project needs, you may not need all the components or pieces included, perhaps, others may be missing.  However, this should give you a great foundation to begin with.

## Project Features

- net 6.0 WebApi
- CI/CD (yaml)
  - Build
  - Analyzers
    - FxCop
    - StyleCop
  - Test
  - Code Coverage (coverlet)
  - Release
    - Uses [GitHub Actions][github-actions]
    - Deploys Azure resources
- Swagger using [NSwag][swagger-nswag]
  - [Swashbuckle][swagger-swashbuckle] is another alternative
  - Navigate to `/swagger` endpoint to view

## Feature Details

### Storage

This project should automatically fetch the Azure Storage Account connection string from Key Vault.  The `RequestLoggerController` is very simple example of to read/write/delete from that storage.  Local development can enable the simulator from configuration to use an in-memory table for testing.  Just set "Features:UseStorageSimulator" to `true`.

[Storage Explorer][storage-explorer] is a great cross-platform utility to help interact with storage accounts during development.

### Configuration

It is not always easy to see in the code, but this project gains a lot from using the [Host.CreateDefaultBuilder][dotnet-configuration-default-builder].  Here is how configuration works out of the box:

- Set the ContentRootPath to the result of GetCurrentDirectory()
- Load host IConfiguration from "DOTNET_" prefixed environment variables
- Load app IConfiguration from 'appsettings.json' and 'appsettings.{_*EnvironmentName*_}.json'
- Load app IConfiguration from User Secrets when EnvironmentName is 'Development' using the entry assembly
- Load app IConfiguration from environment variables
- Configure the ILoggerFactory to log to the console, debug, and event source output
- Enables scope validation on the dependency injection container when EnvironmentName is 'Development'

### Docker

This project comes docker enabled (but not required).  Running in docker is a good way to avoid cluttering your machine with dev tools and frameworks

Building a local image can be done with:

```bash
# Build a local image
docker build -t sample-webapi:local -f ./docker/sample-webapi/Dockerfile .

# Run that image and detch
docker run --publish=8081:80 --rm --detach --name sample-webapi-container sample-webapi:local

# Test it
curl http://localhost:8081/healthcheck

# Stop container
docker stop sample-webapi-container
```

## Best Practices

- [Naming Conventions][naming]
- [Secret Management][developer-secret-management] during development

## Getting Started

For a successful deployment from GitHub Actions, you will need to connect to azure using a service principal.  This can be setup one time and added to both Actions/Codespace secrets in your [GitHub Secrets][githb-secrets].

Tools needed:

- [net 6.0][dotnet-install]
- [az cli][az-cli]

A service principal can be created from the command line by following these steps in bash (minor tweaks for powershell):

```bash
# login from browser
az login --use-device-code

# If you have multiple subscriptions select the one you prefer to deploy into
az account set --subscription "Your Subscription Name"

# Validate
az account show

AZURE_SUBSCRIPTION_ID=$(az account show --query "id" --output tsv)
SERVICE_PRINCIPAL_NAME="github-actions-2"
# Create the sp with contributor role over your subscription (Note: you can limit it down to a specific resource group for tighter access control)
# Take this output for your GitHub secret and save as 'AZURE_CREDENTIALS'
az ad sp create-for-rbac \
    --name $SERVICE_PRINCIPAL_NAME\
    --role contributor \
    --scopes /subscriptions/$AZURE_SUBSCRIPTION_ID \
    --sdk-auth

# Validate
az ad sp list --display-name 'github-actions-for-sample'

SERVICE_PRINCIPAL_ID=$(az ad sp list --display-name $SERVICE_PRINCIPAL_NAME --query "[0].objectId" --output tsv)
ROLE_NAME="Custom Authorization Contributor for $SERVICE_PRINCIPAL_NAME"
# Create role that can assign
az role definition create --role-definition "{
        \"Name\": \"$ROLE_NAME\",
        \"Description\": \"Ability to assign roles\",
        \"Actions\": [
          \"Microsoft.Authorization/roleAssignments/write\"
        ],
        \"DataActions\": [
        ],
        \"NotDataActions\": [
        ],
        \"NotActions\": [
          \"Microsoft.Authorization/*/Delete\"
        ],
        \"AssignableScopes\": [\"/subscriptions/$AZURE_SUBSCRIPTION_ID\"]
    }"

# Validate
az role definition list --name $ROLE_NAME

az role assignment create --assignee $SERVICE_PRINCIPAL_ID --role "$ROLE_NAME"
```

[naming]: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines
[developer-secret-management]: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows
[code-coverage]: https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#collect-code-coverage
[dotnet-configuration]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
[dotnet-configuration-default-builder]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-3.1
[dotnet-install]: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script#examples

[swagger-nswag]: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-3.1&tabs=visual-studio
[swagger-swashbuckle]: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
[storage-explorer]: https://azure.microsoft.com/en-us/features/storage-explorer/
[github-actions]: https://docs.github.com/en/actions/learn-github-actions/understanding-github-actions
[github-secrets]: https://docs.github.com/en/actions/security-guides/encrypted-secrets#creating-encrypted-secrets-for-a-repository
[az-cli]: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli