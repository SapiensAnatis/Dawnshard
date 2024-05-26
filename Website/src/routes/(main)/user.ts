import { z } from 'zod';

export const userSchema = z.object({
  viewerId: z.number().int(),
  name: z.string()
});

export type User = z.infer<typeof userSchema>;
