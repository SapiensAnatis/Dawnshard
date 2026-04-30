import { get } from 'svelte/store';

import { locales } from '$lib/translations';

import type { LayoutServerLoad } from './$types';

function detectLocale(available: string[], request: Request, cookie: string | undefined): string {
  if (cookie && available.includes(cookie)) return cookie;

  const header = request.headers.get('Accept-Language') ?? '';

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

export const load: LayoutServerLoad = ({ request, cookies, depends }) => {
  depends('app:locale');
  const available = get(locales);
  return { locale: detectLocale(available, request, cookies.get('locale')) };
};
