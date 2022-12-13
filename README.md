# DragaliaAPI

[![build](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml)
[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml/badge.svg)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml)

## Setup

1. Install Docker.
2. Configure secrets:
	- Run `dotnet user-secrets init` and `dotnet user-secrets set DeveloperToken <TOKEN>`. This is a token that is required in the headers to authenticate against admin-only controllers (e.g. savefile import).
	- Create a file called `postgres.env` next to the `docker-compose.yml` file with contents as described below, choosing a secure combination for your database username and password.

		```
		POSTGRES_USER=
		POSTGRES_PASSWORD=
		```
3. Build and start the app using `docker compose up -d`.
4. Hopefully, Docker should work its magic; there should be no other dependencies and it should ✨ just work ✨.

Once the server is running, you should be able to make requests to `localhost:5000` (HTTP) or `localhost:5001` (HTTPS) -- these ports can be changed in `docker-compose.yml`.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. Please also include integration-style tests for any new controllers, and unit tests for any new services. See the DragaliaAPI.Test project for examples (these are by no means a gold standard, however).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

### Development environment

If you wish to connect to your locally-hosted instance of the server with a Dragalia client, you will need to set up the workflow for this. Please contact the repository owner for more information. If you simply wish to test individual endpoints in a less end-to-end fashion, you could write automated integration tests or use [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) to send msgpack-formatted requests from your desktop.

