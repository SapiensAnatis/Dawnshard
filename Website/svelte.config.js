import adapter from '@sveltejs/adapter-node';
import nbgv from 'nerdbank-gitversioning';
import { sveltePreprocess } from 'svelte-preprocess'

let version = '0.0.0';

try {
  version = (await nbgv.getVersion()).semVer2;
} catch { /* empty */ }

/** @type {import('@sveltejs/kit').Config} */
const config = {
  // Consult https://kit.svelte.dev/docs/integrations#preprocessors
  // for more information about preprocessors
  preprocess: [sveltePreprocess({
    replace: [['__APP_VERSION__', JSON.stringify(version)],]
  })],

  kit: {
    // See https://kit.svelte.dev/docs/adapters for more information about adapters.
    adapter: adapter(),
    alias: {
      $shadcn: './src/lib/shadcn',
      $static: './static',
      $main: './src/routes/(main)'
    }
  }
};

export default config;
