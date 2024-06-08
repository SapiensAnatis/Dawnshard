# maintenance-worker

This is a Cloudflare Worker written in Rust to show the in-game maintenance alert, as well as a HTML fallback for the main website.

While the existing server can handle maintenance alerts [via MaintenanceOptions](https://github.com/SapiensAnatis/Dawnshard/blob/main/DragaliaAPI/DragaliaAPI/appsettings.json#L113), it cannot do this if it needs to be taken down entirely, or if the container host needs to be restarted. A Cloudflare worker allows us to return maintenance information regardless of the status of the API.

The maintenance data is configured in `wrangler.toml`. Below is an example configuration. All values are required.

```toml
[vars]
MAINTENANCE_END_DATE = "2024-05-20T18:14:08Z" # Must be ISO 8601 format.
MAINTENANCE_TITLE = "Maintenance"
MAINTENANCE_BODY = "Dawnshard is currently under maintenance\nto upgrade the server."
```

## Development

1. Install dependencies:

```bash
pnpm i
```

2. Start the worker in development:

```bash
pnpm run
```

## Deployment 

Follow the Cloudflare documentation for guides on how to deploy the worker:

- https://developers.cloudflare.com/workers/
- https://developers.cloudflare.com/workers/get-started/guide/#4-deploy-your-project

You will need a Cloudflare account. The worker is fairly lightweight and can most likely run within [the free plan](https://developers.cloudflare.com/workers/platform/pricing/#workers).
