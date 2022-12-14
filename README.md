# DragaliaAPI

[![build](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml)
[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml/badge.svg)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml)

## Setup

To deploy the server to a production environment, start by following the steps below:

1. Install Docker.
2. Configure secrets:
	- Run `dotnet user-secrets init` and `dotnet user-secrets set DeveloperToken <TOKEN>`. This is a token that is required in the headers to authenticate against admin-only controllers (e.g. savefile import).
	- Edit the .env file and choose a secure username and password combination for the database.
3. Migrate the database using the EntityFramework SQL script.
	- To generate this, clone the source code and run `dotnet ef migrations script` in the `DragaliaAPI.Database` folder.
4. Build and start the app using `docker compose up -d`.
5. Hopefully, Docker should work its magic; there should be no other dependencies and it should ✨ just work ✨.

Once the server is running, you should be able to make requests to `localhost:5000` (HTTP) or `localhost:5001` (HTTPS) -- these ports can be changed in `docker-compose.yml`.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. Please also include integration-style tests for any new controllers, and unit tests for any new services. See the DragaliaAPI.Test project for examples (these are by no means a gold standard, however).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

### Development environment

To set up a development environment, the steps are similar to setting up a deployment, but it may be more convenient to generate a code migration instead by using `dotnet ef migrations add <migration name>`, as these are automatically applied on app startup when running in development mode.

If you wish to connect to your locally-hosted instance of the server with a Dragalia client, you will need to set up the workflow for this. Please contact the repository owner for more information. If you simply wish to test individual endpoints in a less end-to-end fashion, you could write automated integration tests or use [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) to send msgpack-formatted requests from your desktop.

