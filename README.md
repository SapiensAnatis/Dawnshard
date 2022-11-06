# DragaliaAPI

[![build](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/build.yml)
[![test](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml/badge.svg)](https://github.com/SapiensAnatis/DragaliaAPI/actions/workflows/test.yml)

## Setup

1. Install Docker Desktop.
2. For security, change the database password in `docker-compose.yml`, and then update the connection string in `appsettings.json` with the new password. You may also wish to change the default password hashing salt in `appsettings.json`.
3. Build and start the app using `docker-compose up --build --force-recreate dragaliaapi`
4. Hopefully, Docker should work its magic; there should be no other dependencies and it should ✨ just work ✨.

Once the server is running, you should be able to make requests to `localhost:5000` (HTTP) or `localhost:5001` (HTTPS) -- these ports can be changed in `docker-compose.yml`.

## Contributing

Contributions are more than welcome! Feel free to fork the repository and open a pull request with these changes. Please also include integration-style tests for any new controllers, and unit tests for any new services. See the DragaliaAPI.Test project for examples (these are by no means a gold standard, however).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.

### Development environment

If you wish to connect to your locally-hosted instance of the server with a Dragalia client, you will need to set up the workflow for this. Please contact the repository owner for more information. If you simply wish to test individual endpoints in a less end-to-end fashion, you could write automated integration tests or use [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) to send msgpack-formatted requests from your desktop.

