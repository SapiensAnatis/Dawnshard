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

export type PresentFormSubmission = {
  type: EntityType;
  item: number;
  itemLabel: string;
  quantity: number;
};

export type SaveChangesRequest = {
  presents: PresentFormSubmission[];
};

export type PresentWidgetData = z.infer<typeof presentWidgetDataSchema>;
