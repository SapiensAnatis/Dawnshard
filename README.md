# DragaliaAPI

[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml/badge.svg?branch=develop)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml)

DragaliaAPI is a server emulator for Dragalia Lost.

You can play using the [Dragalipatch](https://github.com/lukeFZ/dragalipatch) app by LukeFZ to redirect server traffic to https://dragalia.sapiensanatis.com.

If you haven't already, please also consider joining the Dragalia Lost Reverse Engineering [Discord server](https://discord.gg/j9zSttjjWj); this is where development is discussed and where bugs/issues are most easily reported.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. 

For guidance on contributing, including the process for setting up a development environment, please see the [GitHub Wiki](https://github.com/SapiensAnatis/DragaliaAPI/wiki).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

## Deploying to production

If you are interested in hosting your own instance, it is recommended to use the [Kubernetes helm chart](https://github.com/SapiensAnatis/helm-charts). 

The application is deployed as three services: the main ASP.NET service which is stateless, and two stateful services in Redis (session management) and PostgreSQL (savefile storage). 
