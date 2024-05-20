# maintenance-worker

This is a Cloudflare Worker written in Rust to show the in-game maintenance alert, as well as a HTML fallback for the main website.

While the existing server can handle maintenance alerts [via MaintenanceOptions](https://github.com/SapiensAnatis/Dawnshard/blob/main/DragaliaAPI/DragaliaAPI/appsettings.json#L113), it cannot do this if it needs to be taken down entirely, or if the container host needs to be restarted. A Cloudflare worker allows us to return maintenance information regardless of the status of the API.

The maintenance data is configured in `wrangler.toml`, e.g.

```toml
[vars]
MAINTENANCE_END_DATE = "2024-05-20T18:14:08Z"
MAINTENANCE_TITLE = "Maintenance"
MAINTENANCE_BODY = "Dawnshard is currently under maintenance\nto upgrade the server."
```
