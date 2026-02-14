import { enhancedImages } from '@sveltejs/enhanced-img';
import { sveltekit } from '@sveltejs/kit/vite';
import tailwindcss from '@tailwindcss/vite';
import { defineConfig, loadEnv } from 'vite';
import checker from 'vite-plugin-checker';

export default defineConfig(({ mode }) => {
  process.env = { ...process.env, ...loadEnv(mode, process.cwd(), '') };

  return {
    plugins: [
      enhancedImages(),
      sveltekit(),
      checker({
        typescript: false, // https://github.com/huntabyte/shadcn-svelte/issues/1468
        eslint: { lintCommand: 'eslint ./src/**/*.{ts,svelte}', useFlatConfig: true }
      }),
      tailwindcss()
    ],
    server: {
      port: 3001,
      host: true,
      proxy: {
        '/api': {
          target: process.env.DAWNSHARD_API_URL_SSR,
          changeOrigin: true
        }
      }
    },
    build: {
      // Hack to get top-level await support required by Mock Service Worker for Playwright
      target: mode === 'development' ? 'es2022' : 'baseline-widely-available'
    },
    preview: {
      port: 3001,
      proxy: {
        '/api': {
          target: process.env.DAWNSHARD_API_URL_SSR,
          changeOrigin: true
        }
      }
    }
  };
});
