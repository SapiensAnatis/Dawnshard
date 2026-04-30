import { loadTranslations } from '$lib/translations';

import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ url, data }) => {
  await loadTranslations(data.locale, url.pathname);
  return {};
};
