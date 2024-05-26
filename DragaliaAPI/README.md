# DragaliaAPI

DragaliaAPI is the main server component of Dawnshard, which handles the vast majority of game functionality.

## Dependencies

The server depends on [`DragaliaBaas`](https://github.com/DragaliaLostRevival/DragaliaBaasServer) as an identity
provider. Clients are expected to go to an instance of the BaaS for login and authentication, and then come back
to `/tool/auth` with a signed JSON web token to authenticate against DragaliaAPI.

## Development environment

### Run the server

To get started, copy the `.env.default` file to `.env`. Choose some values for the database credentials, and then launch
the compose project from your IDE. Or, if using the command line,
use `docker-compose -f docker-compose.yml -f docker-compose.override.yml --profiles dragaliaapi`.

The solution includes a `docker-compose.dcproj` project file which should be plug-and-play with Visual Studio and allow
launching the API plus the supporting Postgres and Redis services. It is compatible with container fast mode, so you can
iterate during development without rebuilding the containers each time. Other IDEs, including JetBrains Rider, should
also able to use the `docker-compose.yml` file if you add a run configuration pointed at it (as well
as `docker-compose.override.yml`). For users who are not using Visual Studio, ensure that your `docker-compose`
configuration or command includes an instruction to use the `dragaliaapi` profile so that the API is launched.

If you have issues with using the container fast mode, you can use the docker-compose file to only launch the supporting
services and then run the API directly on your machine. Either remove the profile arguments in your IDE or just
run `docker-compose -f docker-compose.yml up -d` from the command line without any `--profile` arguments to start Redis
and Postgres, and then launch the main project. You will need to configure the environment variables that it is run with
to match what is set in `docker-compose.yml`, and also to adjust the hostnames of Redis and Postgres now that it is not
running in the container network. 

An example configuration for running outside a container which is supported by Rider, Visual Studio, and the `dotnet` 
cli, is included in [launchSettings.json](./DragaliaAPI/Properties/launchSettings.json). It does not include credentials
as it is in source control. The recommended way to set the credentials is using user secrets. See the [Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets#secret-manager) documentation on user secrets for 
more information. 

You will need to set the following values corresponding to the values being used by Docker in `.env`:

- `PostgresOptions:Username`
- `PostgresOptions:Password` 
- `PostgresOptions:Database` 


### Set up a client

The `docker-compose.yml` / `launchSettings.json` file will start the server on port 80, so you can
use [Dragalipatch](https://github.com/LukeFZ/DragaliPatch/releases/latest) with
your [PC's local IP address](https://support.microsoft.com/en-us/windows/find-your-ip-address-in-windows-f21a9bbc-c582-55cd-35e0-73431160a1b9)
to play on your local server with an emulator or mobile device. You must input the local IP address
as `http://192.168.xxx.xxx` because without a http prefix, Dragalipatch assumes HTTPS which is not enabled on the
development setup.

## Self-hosting for general use

### Locally

The recommended way to self-host is using `docker-compose` -- please see
the [self-hosting guide](https://github.com/SapiensAnatis/Dawnshard/wiki/Self-hosting-guide) in the wiki for more
information.

### Dedicated server

On a dedicated server, the basic `docker-compose` setup will work, but additional considerations should be made
regarding reverse proxying, logging, etc. Speak to the maintainer if you are interested in hosting your own instance for
further guidance.


