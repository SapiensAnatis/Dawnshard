import { z } from 'zod';

export const userProfileSchema = z.object({
  lastSaveImportTime: z
    .string()
    .datetime({ offset: true })
    .transform((val) => new Date(val))
    .nullable(),
  lastLoginTime: z
    .string()
    .datetime({ offset: true })
    .transform((val) => new Date(val)),
  settings: z.object({
    dailyGifts: z.boolean()
  })
});

export type UserProfile = z.infer<typeof userProfileSchema>;
