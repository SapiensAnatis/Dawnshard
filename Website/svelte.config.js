import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

const scriptCsp = [
  'self',
  // https://github.com/sveltejs/svelte/issues/14014
  'unsafe-hashes',
  'sha256-7dQwUgLau1NFCCGjfn9FsYptB6ZtWxJin6VohGIu20I='
];

/** @type {import('@sveltejs/kit').Config} */
const config = {
  // Consult https://kit.svelte.dev/docs/integrations#preprocessors
  // for more information about preprocessors
  preprocess: [vitePreprocess()],
  kit: {
    // See https://kit.svelte.dev/docs/adapters for more information about adapters.
    adapter: adapter(),
    alias: {
      $shadcn: './src/lib/shadcn',
      $static: './static',
      $main: './src/routes/(main)'
    },
    csp: {
      directives: {
        'script-src': scriptCsp
      },
      reportOnly: {
        'script-src': scriptCsp,
        'report-uri': ['/csp']
      }
    }
  }
};

export default config;
