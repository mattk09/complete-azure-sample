# Introduction

This project is meant to be a general starting point for most common dotnet projects, provides a boiler plate code for basic core engineering fundamentals, like observability, testing, security, and CI/CD, including industry known best practices.

Depending on your project needs, you may not need all components or pieces included, perhaps, others may be missing, however, this should give you a great foundation to begin with.

## Project Features

- net 6.0 WebApi
- CI/CD (all yaml/stages)
  - Build
  - Analyzers
    - FxCop
    - StyleCop
    - Document spell check and Markdown Lint
  - Test
  - Code Coverage (coverlet)
  - Release
    - Deploys Azure resources
    - Uses environments
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

## Best Practices

- [Naming Conventions][naming]
- [Secret Management][developer-secret-management] during development

[naming]: https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines
[developer-secret-management]: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows
[code-coverage]: https://docs.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops#collect-code-coverage
[dotnet-configuration]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
[dotnet-configuration-default-builder]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-3.1

[swagger-nswag]: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-3.1&tabs=visual-studio
[swagger-swashbucke]: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
[storage-explorer]: https://azure.microsoft.com/en-us/features/storage-explorer/
