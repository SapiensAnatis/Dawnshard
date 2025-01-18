import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

// An ideal script-src policy:
// - would remove unsafe-hashes:
//   https://github.com/sveltejs/svelte/issues/14014
// - would use strict-dynamic + unsafe-inline for backwards compatibility:
//   https://github.com/sveltejs/kit/issues/3558
// - would use trusted-types-for
//   https://github.com/sveltejs/svelte/issues/14438
const csp = Object.freeze({
  'script-src': ['self', 'unsafe-hashes', 'sha256-7dQwUgLau1NFCCGjfn9FsYptB6ZtWxJin6VohGIu20I='],
  'object-src': ['none'],
  'base-uri': ['none']
});

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
        ...csp
      },
      reportOnly: {
        ...csp,
        'report-uri': ['/csp']
      },
      mode: 'nonce'
    }
  }
};

export default config;
