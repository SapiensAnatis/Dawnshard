import { defineI18n } from 'sveltekit-i18n/kit';

import { dev } from '$app/environment';

export const translations: Partial<Record<string, string>> = Object.freeze({
  en: 'English',
  'zh-CN': '中文 (partial)'
});

const config = {
  log: {
    level: dev ? 'warn' : 'error'
  },
  translations: {
    en: translations,
    'zh-CN': translations
  },
  loaders: [
    {
      locale: 'en',
      namespace: 'timeAttack',
      routes: [/\/events\/time-attack\/rankings\/.*/],
      loader: async () => (await import('./en/time-attack.json')).default
    },
    {
      locale: 'en',
      namespace: 'entity',
      routes: [/\/events\/time-attack\/rankings\/.*/, '/account/save-editor'],
      loader: async () => (await import('./en/entity.json')).default
    },
    {
      locale: 'zh-CN',
      namespace: 'entity',
      routes: [/\/events\/time-attack\/rankings\/.*/, '/account/save-editor'],
      loader: async () => (await import('./zh-CN/entity.json')).default
    }
  ],
  initLocale: 'en',
  fallbackLocale: 'en'
} as const;

const available = Object.keys(translations);

function detectLocale(cookie: string | undefined, request: Request | undefined): string {
  if (cookie && available.includes(cookie)) return cookie;

  const header = request?.headers.get('Accept-Language') ?? '';

  const preferred = header
    .split(',')
    .map((p) => {
      const [lang, q] = p.trim().split(';q=');
      const qFloat = parseFloat(q);
      return { lang: lang.trim(), q: Number.isFinite(qFloat) ? qFloat : 1.0 };
    })
    .sort((a, b) => b.q - a.q);

  for (const { lang } of preferred) {
    if (available.includes(lang)) return lang;
    const prefix = lang.split('-')[0];
    const match = available.find((l) => l.split('-')[0] === prefix);
    if (match) return match;
  }

  return 'en';
}

export const { handle, load, use, get } = defineI18n(config, {
  preferredLocale: (event) => detectLocale(event.cookies?.get('locale'), event.request)
});
