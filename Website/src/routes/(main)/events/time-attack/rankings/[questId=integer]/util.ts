import type { TimeAttackPlayer } from '../timeAttackTypes.ts';

export const getTeam = (coop: boolean, players: TimeAttackPlayer[]) => {
  return coop ? players.map((p, i) => ({ ...p.units[0], position: i })) : players[0].units;
};

export const getTeamKeys = (coop: boolean, players: TimeAttackPlayer[]) => {
  return coop ? players.map((p) => p.name) : players[0].units.map((_, i) => `Unit ${i + 1}`);
};
