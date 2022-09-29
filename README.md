# DragaliaAPI

## Setup

1. Install Docker Desktop
2. For security, change the database password in `docker-compose.yml`, and then update the connection string in `appsettings.json` with the new password. You may also wish to change the default password hashing salt in `appsettings.json`.
3. Build and start the app using `docker-compose up --build --force-recreate dragaliaapi`
4. Hopefully, Docker should work its magic; there should be no other dependencies and it should ✨ *just work* ✨.

Once the server is running, you should be able to make requests to `localhost:5000` (HTTP) or `localhost:5001` (HTTPS) -- these can be changed in `docker-compose.yml`.

For development purposes, you may find [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) useful for sending requests to endpoints that expect msgpack-formatted bodies (as Postman does not yet support this).

See also the [API documentation](https://dragalia-api-docs.readthedocs.io/en/latest/) for reference on what existing endpoints do and how to implement new ones.
