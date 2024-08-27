import i18n, { type Config } from 'sveltekit-i18n';

import { dev } from '$app/environment';

export const defaultLocale = 'en';

const config: Config = {
  log: {
    level: dev ? 'warn' : 'error'
  },
  translations: {
    en: { en: 'English' }
  },
  loaders: [
    {
      locale: 'en',
      key: 'saveEditor',
      routes: ['/account/save-editor'],
      loader: async () => (await import('./en/save-editor.json')).default
    }
  ],
  fallbackLocale: 'en'
};

export const { t, locale, locales, loading, loadTranslations } = new i18n(config);
