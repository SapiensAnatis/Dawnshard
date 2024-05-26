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

## Authentication

The website uses OAuth with [`DragaliaBaas`](https://github.com/DragaliaLostRevival/DragaliaBaasServer) as an identity provider. When a user logs in, they are redirected to the BaaS to authorize their linked account, and given an JSON web token as a cookie which can be used to authenticate against the main server.
