import { z } from 'zod';

export const impersonationSessionSchema = z.object({
  impersonatedViewerId: z.number().nullable()
});
