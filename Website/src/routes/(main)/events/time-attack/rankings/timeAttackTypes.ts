import { z } from 'zod';

export const questListSchema = z.number().int().array();

const talisman = z.object({
  id: z.number().int(),
  element: z.number().int(),
  weaponType: z.number().int(),
  ability1Id: z.number().int().nullable(),
  ability2Id: z.number().int().nullable()
});

const abilityCrest = z.object({
  id: z.number().int(),
  baseId: z.number(),
  imageNum: z.number().int()
});

const baseIdEntity = z.object({ id: z.number(), baseId: z.number(), variationId: z.number() });

const playerUnitSchema = z.object({
  chara: baseIdEntity,
  dragon: baseIdEntity.nullable(),
  weapon: baseIdEntity.extend({ formId: z.number().int(), changeSkillId1: z.number().int() }), // send wooden weapons instead of null
  talisman: talisman.nullable(),
  crests: abilityCrest.nullable().array(),
  sharedSkills: z.object({ id: z.number().int(), skillLv4IconName: z.string() }).array()
});

export type TimeAttackUnit = z.infer<typeof playerUnitSchema>;

const playerSchema = z.object({
  name: z.string(),
  units: playerUnitSchema.array()
});

export type TimeAttackPlayer = z.infer<typeof playerSchema>;

const timeAttackClearSchema = z.object({
  rank: z.number().int(),
  time: z.number(),
  players: playerSchema.array()
});

export const timeAttackClearArraySchema = timeAttackClearSchema.array();

export type TimeAttackClear = z.infer<typeof timeAttackClearSchema>;
