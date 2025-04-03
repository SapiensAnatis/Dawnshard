import { z } from 'zod';

export const EntityType = z.enum([
  'Chara',
  'Item',
  'Dragon',
  'Material',
  'DmodePoint',
  'SkipTicket',
  'DragonGift',
  'FreeDiamantium',
  'Wyrmite',
  'HustleHammer',
  'Dew',
  'Rupies',
  'Wyrmprint',
  'WeaponBody',
  'WeaponSkin'
]);

export type EntityType = z.infer<typeof EntityType>;

const item = z.object({
  id: z.number().int()
});

export const presentWidgetDataSchema = z.object({
  types: z
    .object({
      type: EntityType,
      maxQuantity: z.number().int()
    })
    .array(),
  availableItems: z.record(EntityType, item.array())
});

export type PresentFormSubmission = {
  id: number;
  type: EntityType;
  item: number;
  quantity: number;
};

export type SaveChangesRequest = {
  presents: PresentFormSubmission[];
};

export type PresentWidgetData = z.infer<typeof presentWidgetDataSchema>;
