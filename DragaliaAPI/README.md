# DragaliaAPI

DragaliaAPI is the main server component of Dawnshard, which handles the vast majority of game functionality.

## Hosting your own instance

The application is deployed as three services: the main ASP.NET service which is stateless, and two stateful services in Redis (session management) and PostgreSQL (savefile storage).

### Locally

The recommended way to self-host (for personal use or development) is using `docker-compose` -- please see the [self-hosting guide](https://github.com/SapiensAnatis/Dawnshard/wiki/Self-hosting-guide) in the wiki for more information.

### Dedicated server

On a dedicated server, the basic `docker-compose` setup will work, but additional considerations should be made regarding reverse proxying, logging, etc. Speak to the maintainer if you are interested in hosting your own instance for further guidance.
