# Website

The website is a new frontend for Dawnshard that serves as the public-facing home for the project. It also allows players to view the news, time attack rankings, or administer their account by offering e.g. save export.

It uses SvelteKit 4 and is deployed as a server-side-rendered web app with [`adapter-node`](https://kit.svelte.dev/docs/adapter-node).

The project uses `pnpm` to manage packages locally, and has code style enforced by `prettier` and `eslint`.

## Development

To set up this project locally, first install dependencies:

```bash
pnpm i
```

Then start the Vite dev server:

```bash
pnpm run dev
```

### Tests

Some of the Playwright tests which require a login to BaaS make use of private environment variables to know which credentials to use. To run these tests, make a file called `.env.local` in `./tests/` with the following content:

```text
BAAS_USERNAME=your_username_here
BAAS_PASSWORD=your_password_here
```

These must be a valid login to the real https://baas.lukefz.xyz/ website. It is not required for them to have any Dawnshard save data.
## Authentication

The website uses OAuth with [`DragaliaBaas`](https://github.com/DragaliaLostRevival/DragaliaBaasServer) as an identity provider. When a user logs in, they are redirected to the BaaS to authorize their linked account, and given an JSON web token as a cookie which can be used to authenticate against the main server.

