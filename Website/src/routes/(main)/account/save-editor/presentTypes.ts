import { z } from 'zod';

export const EntityType = z.enum(['Material', 'Chara', 'DmodePoint']);

export type EntityType = z.infer<typeof EntityType>;

const item = z.object({
  value: z.number().int(),
  label: z.string()
});

export const presentWidgetDataSchema = z.object({
  types: z
    .object({
      value: EntityType,
      label: z.string(),
      hasQuantity: z.boolean()
    })
    .array(),
  availableItems: z.record(EntityType, item.array())
});

export type PresentWidgetData = z.infer<typeof presentWidgetDataSchema>;

export const presentFormSchema = z.object({
  type: EntityType,
  item: z.number().int(),
  quantity: z.number().int().min(1).max(2147483647)
});

export type PresentFormSchema = typeof presentFormSchema;
