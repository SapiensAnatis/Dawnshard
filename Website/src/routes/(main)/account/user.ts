import { z } from 'zod';

export const userSchema = z.object({
  viewerId: z.number().positive(),
  name: z.string(),
  claims: z.record(z.string(), z.string())
});

export type User = z.infer<typeof userSchema>;
