import { z } from 'zod';

export const EntityType = z.enum([
  'Chara',
  'Dragon',
  'Item',
  'Material',
  'DmodePoint',
  'SkipTicket',
  'DragonGift'
]);

export type EntityType = z.infer<typeof EntityType>;

const item = z.object({
  id: z.number().int()
});

export const presentWidgetDataSchema = z.object({
  types: z
    .object({
      type: EntityType,
      hasQuantity: z.boolean()
    })
    .array(),
  availableItems: z.record(EntityType, item.array())
});

export type PresentFormSubmission = {
  type: EntityType;
  item: number;
  quantity: number;
};

export type SaveChangesRequest = {
  presents: PresentFormSubmission[];
};

export type PresentWidgetData = z.infer<typeof presentWidgetDataSchema>;
