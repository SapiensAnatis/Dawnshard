# DragaliaAPI

## Setup

1. Install Docker Desktop
2. For security, change the database password in `docker-compose.yml`, and then update the connection string in `appsettings.json` with the new password.
3. Hopefully, Docker should work its magic there should be no other dependencies and it should Just Work (TM).

Once the server is running, you should be able to make requests to `localhost:5000` or `localhost:5001` (or whichever ports you set in `docker-compose.yml`)

For development purposes, you may find [this tool](https://gist.github.com/SapiensAnatis/e76f067aad0ac425c9f9008db94e143c) useful for sending requests and inspecting responses.
