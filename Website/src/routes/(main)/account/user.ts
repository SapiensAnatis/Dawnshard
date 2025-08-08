import { z } from 'zod';

export const userSchema = z.object({
  viewerId: z.number().positive(),
  name: z.string(),
  isAdmin: z.boolean()
});

export type User = z.infer<typeof userSchema>;
