import { enhancedImages } from '@sveltejs/enhanced-img';
import { sveltekit } from '@sveltejs/kit/vite';
import nbgv from 'nerdbank-gitversioning';
import { defineConfig } from 'vite';
import checker from 'vite-plugin-checker';

let version = '0.0.0';

try {
  version = (await nbgv.getVersion()).semVer2;
} catch (error) {
  console.error('Failed to get version', error);
}

console.log('version', version);

export default defineConfig(({ mode }) => ({
  plugins: [
    sveltekit(),
    enhancedImages(),
    checker({
      overlay: true, // Doesn't work
      typescript: true,
      eslint: { lintCommand: 'eslint ./src/**/*.{ts,svelte}', useFlatConfig: true }
    })
  ],
  server: {
    port: 3001,
    host: true
  },
  build: {
    // Hack to get top-level await support required by Mock Service Worker for Playwright
    target: mode === 'development' ? 'es2022' : 'modules'
  },
  preview: {
    port: 3001,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  },
  define: {
    __APP_VERSION__: JSON.stringify(version)
  }
}));
