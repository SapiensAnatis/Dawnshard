import { z } from 'zod';

import { page } from '$app/state';
import { PUBLIC_CDN_URL } from '$env/static/public';

export type NewsItem = z.infer<typeof newsItemSchema>;

export const pageSize = 5;
export const lastReadKey = 'lastReadNews';

export const newsItemSchema = z.object({
  id: z.number(),
  headline: z.string(),
  description: z.string(),
  date: z
    .iso
    .datetime({ offset: true })
    .transform((val) => new Date(val)),
  headerImagePath: z.string().nullable(),
  headerImageAltText: z.string().nullable(),
  bodyImagePath: z.string().nullable(),
  bodyImageAltText: z.string().nullable()
});

export const newsSchema = z.object({
  pagination: z.object({
    totalCount: z.number()
  }),
  data: newsItemSchema.array()
});

export const makeRequestUrl = (pageNo: number, urlOrigin: string) => {
  const offset = (pageNo - 1) * pageSize;
  const query = new URLSearchParams({
    offset: offset.toString(),
    pageSize: pageSize.toString()
  });
  return new URL(`/api/news?${query}`, urlOrigin);
};

export const getImageSrc = (path: string | null) =>
  path ? new URL(path, PUBLIC_CDN_URL).href : undefined;

export const getPageNoFromParams = (params: URLSearchParams) => {
  const pageNoStr = params.get('page') || '1';
  return Number.parseInt(pageNoStr) || 1;
};

export const formatDescription = (description: string) => {
  return description.replace("{{Hostname}}", page.url.hostname);
}