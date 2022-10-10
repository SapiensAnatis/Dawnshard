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

Code formatting is enforced by [CSharpier](https://csharpier.com/) with a pre-commit hook and a pipeline step. You may find it beneficial to run this on saving a file -- this can be done using an extension in your IDE. For Visual Studio, install [this extension](https://marketplace.visualstudio.com/items?itemName=csharpier.CSharpier) and enable 'Reformat with CSharpier on Save' in Options > CSharpier > General.

For development purposes, you may find [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) useful for sending requests to endpoints that expect msgpack-formatted bodies (as Postman does not yet support this).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.
