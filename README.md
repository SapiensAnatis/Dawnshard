# Dawnshard

[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml/badge.svg?branch=develop)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yaml)

Dawnshard (internally named DragaliaAPI) is a server emulator for Dragalia Lost.

You can play using the [Dragalipatch](https://github.com/lukeFZ/dragalipatch) app by LukeFZ to redirect server traffic to https://dawnshard.co.uk.

If you haven't already, please also consider joining the Dragalia Lost Reverse Engineering [Discord server](https://discord.gg/j9zSttjjWj); this is where development is discussed and where bugs/issues are most easily reported.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes.

For guidance on contributing, including the process for setting up a development environment, please see the [GitHub Wiki](https://github.com/SapiensAnatis/Dawnshard/wiki).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

## Hosting your own instance

The application is deployed as three services: the main ASP.NET service which is stateless, and two stateful services in Redis (session management) and PostgreSQL (savefile storage).

### Locally

The recommended way to self-host (for personal use or development) is using `docker-compose` -- please see the [self-hosting guide](https://github.com/SapiensAnatis/Dawnshard/wiki/Self-hosting-guide) in the wiki for more information.

### Dedicated server

On a dedicated server, the basic `docker-compose` setup will work, but additional considerations should be made regarding reverse proxying, logging, etc. Speak to the maintainer if you are interested in hosting your own instance for further guidance.

## Acknowledgements

A big thanks to JetBrains for providing open source licenses for this project.
