# Dawnshard

[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml/badge.svg?branch=develop)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml) [![Codecov](https://img.shields.io/codecov/c/github/sapiensanatis/dawnshard?logo=codecov)](https://app.codecov.io/gh/SapiensAnatis/Dawnshard) [![Docker Image Version (latest by date)](https://img.shields.io/docker/v/sapiensanatis/dragalia-api?label=docker%20image&logo=docker)](https://hub.docker.com/r/sapiensanatis/dragalia-api)

Dawnshard (internally named DragaliaAPI) is a server emulator for Dragalia Lost.

You can play using the [Dragalipatch](https://github.com/lukeFZ/dragalipatch) app by LukeFZ to redirect server traffic to https://dawnshard.co.uk.

If you haven't already, please also consider joining the Dragalia Lost Reverse Engineering [Discord server](https://discord.gg/j9zSttjjWj); this is where development is discussed and where bugs/issues are most easily reported.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. 

For guidance on contributing, including the process for setting up a development environment, please see the [GitHub Wiki](https://github.com/SapiensAnatis/Dawnshard/wiki).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

## Hosting your own instance

- On a dedicated server, it is recommended to use the [Kubernetes helm chart](https://github.com/SapiensAnatis/helm-charts). The application is deployed as three services: the main ASP.NET service which is stateless, and two stateful services in Redis (session management) and PostgreSQL (savefile storage). 
- If you don't want to use Kubernetes, or are looking to host a local instance, you can use the docker-compose.yml file here with [the published Docker image](https://hub.docker.com/repository/docker/sapiensanatis/dragalia-api/general).
- If you don't want to use Docker at all, see the [no-docker branch](https://github.com/sapiensAnatis/dragaliaAPI/tree/no-docker) which uses an SQLite DB and an in-memory IDistributedCache for session management. Please note that this version is not regularly updated and no guarantees are made that it functions correctly.

