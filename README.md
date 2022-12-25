# DragaliaAPI

[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml/badge.svg)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml)

DragaliaAPI is a server emulator for Dragalia Lost.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. Please also include integration-style tests for any new controllers, and unit tests for any new services. See the DragaliaAPI.Test project for examples (these are by no means a gold standard, however).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

### Development environment

To set up a development environment, the steps are similar to setting up a deployment, but it may be more convenient to generate a code migration instead by using `dotnet ef migrations add <NAME> -s DragaliaAPI\DragaliaAPI.csproj -p DragaliaAPI.Database\DragaliaAPI.Database.csproj`, as these are automatically applied on app startup when running in development mode.

If you wish to connect to your locally-hosted instance of the server with a Dragalia client, you will need to set up the workflow for this. Please contact the repository owner for more information. If you simply wish to test individual endpoints in a less end-to-end fashion, you could write automated integration tests or use [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) to send msgpack-formatted requests from your desktop.

## Deploying to production

If you are interested in hosting your own instance, it is recommended to use the [Kubernetes helm chart](https://github.com/SapiensAnatis/helm-charts). 

The application is deployed as three services: the main ASP.NET service which is stateless, and two stateful services in Redis (session management) and PostgreSQL (savefile storage). 