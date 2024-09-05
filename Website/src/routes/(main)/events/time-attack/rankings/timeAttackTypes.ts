import { z } from 'zod';

const questSchema = z.object({ id: z.number().int(), isCoop: z.boolean() });

export const questArraySchema = questSchema.array();

export type TimeAttackQuest = z.infer<typeof questSchema>;

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

const sharedSkill = z.object({ id: z.number().int(), skillLv4IconName: z.string() });

const baseIdEntity = z.object({ id: z.number(), baseId: z.number(), variationId: z.number() });

const weapon = baseIdEntity.extend({ formId: z.number().int(), changeSkillId1: z.number().int() }); // send wooden weapons instead of null

const playerUnitSchema = z.object({
  position: z.number().int(),
  chara: baseIdEntity,
  dragon: baseIdEntity.nullable(),
  weapon: weapon,
  talisman: talisman.nullable(),
  crests: abilityCrest.nullable().array(),
  sharedSkills: sharedSkill.array()
});

export type TimeAttackUnit = z.infer<typeof playerUnitSchema>;

const playerSchema = z.object({
  name: z.string(),
  units: playerUnitSchema.array()
});

export type TimeAttackPlayer = z.infer<typeof playerSchema>;

const timeAttackRankingSchema = z.object({
  rank: z.number().int(),
  time: z.number(),
  players: playerSchema.array()
});

export const timeAttackClearArraySchema = z.object({
  pagination: z.object({
    totalCount: z.number()
  }),
  data: timeAttackRankingSchema.array()
});

export type TimeAttackRanking = z.infer<typeof timeAttackRankingSchema>;
export type TimeAttackRankingResponse = z.infer<typeof timeAttackClearArraySchema>;
