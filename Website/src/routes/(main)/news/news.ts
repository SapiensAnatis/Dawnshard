import { z } from 'zod';

export type NewsItem = z.infer<typeof newsSchema>['data'][0];

export const pageSize = 5;
export const lastReadKey = 'lastReadNews';

export const newsItemSchema = z.object({
  id: z.number(),
  headline: z.string(),
  description: z.string(),
  date: z
    .string()
    .datetime({ offset: true })
    .transform((val) => new Date(val)),
  headerImageSrc: z.string().url().nullable(),
  bodyImageSrc: z.string().url().nullable()
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
