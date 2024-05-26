# DragaliaAPI

DragaliaAPI is the main server component of Dawnshard, which handles the vast majority of game functionality.

## Dependencies

The server depends on [`DragaliaBaas`](https://github.com/DragaliaLostRevival/DragaliaBaasServer) as an identity provider. Clients are expected to go to an instance of the BaaS for login and authentication, and then come back to `/tool/auth` with a signed JSON web token to authenticate against DragaliaAPI.

## Development environment

To get started, copy the `.env.default` file to `.env`. Choose some values for the database credentials, and then launch
the compose project from your IDE. Or, if using the command line,
use `docker-compose -f docker-compose.yml -f docker-compose.override.yml --profiles dragaliaapi`.

The solution includes a `docker-compose.dcproj` project file which should be plug-and-play with Visual Studio and allow
launching the API plus the supporting Postgres and Redis services. It is compatible with container fast mode, so you can
iterate during development without rebuilding the containers each time. Other IDEs, including JetBrains Rider, should
also able to use the `docker-compose.yaml` file if you add a run configuration pointed at it (as well
as `docker-compose.override.yaml`). For users who are not using Visual Studio, ensure that your `docker-compose`
configuration or command includes an instruction to use the `dragaliaapi` profile so that the API is launched.

If you have issues with using the container fast mode, you can use the docker-compose file to only launch the supporting
services and then run the API directly on your machine. Either remove the profile arguments in your IDE or just
run `docker-compose -f docker-compose.yaml up -d` from the command line without any `--profile` arguments to start Redis
and Postgres, and then launch the main project. You will need to configure the environment variables that it is run with
to match what is set in `docker-compose.yaml`, and also to adjust the hostnames of Redis and Postgres now that it is not
running in the container network. An example configuration for this which hopefully should work with Rider and with
Visual Studio, as well as the `dotnet` cli, is included in
[launchSettings.json](./DragaliaAPI/Properties/launchSettings.json). You will need to populate the credentials with the
values that the Docker containers are using from `.env`.

## Self-hosting for general use

### Locally

The recommended way to self-host is using `docker-compose` -- please see
the [self-hosting guide](https://github.com/SapiensAnatis/Dawnshard/wiki/Self-hosting-guide) in the wiki for more
information.

### Dedicated server

On a dedicated server, the basic `docker-compose` setup will work, but additional considerations should be made
regarding reverse proxying, logging, etc. Speak to the maintainer if you are interested in hosting your own instance for
further guidance.


