import i18n, { type Config } from 'sveltekit-i18n';

import { dev } from '$app/environment';

interface Params {
  element: number;
  weaponType: number;
}

export const defaultLocale = 'en';

const config: Config<Params> = {
  log: {
    level: dev ? 'warn' : 'error'
  },
  translations: {
    en: { en: 'English' },
    'zh-CN': { 'zh-CN': '中文' }
  },
  loaders: [
    {
      locale: 'en',
      key: 'timeAttack',
      routes: [/\/events\/time-attack\/rankings\/.*/],
      loader: async () => (await import('./en/time-attack.json')).default
    },
    {
      locale: 'en',
      key: 'entity',
      routes: [/\/events\/time-attack\/rankings\/.*/, '/account/save-editor'],
      loader: async () => (await import('./en/entity.json')).default
    },
    {
      locale: 'zh-CN',
      key: 'entity',
      routes: [/\/events\/time-attack\/rankings\/.*/, '/account/save-editor'],
      loader: async () => (await import('./zh-CN/entity.json')).default
    }
  ],
  fallbackLocale: 'en'
};

export const { t, l, locale, locales, loading, loadTranslations } = new i18n(config);
