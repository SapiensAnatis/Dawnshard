import { enhancedImages } from '@sveltejs/enhanced-img';
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import checker from 'vite-plugin-checker';

export default defineConfig({
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
    port: 3001
  }
});
