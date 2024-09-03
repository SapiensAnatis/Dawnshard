import { PUBLIC_CDN_URL } from '$env/static/public';

import type { TimeAttackPlayer } from '../timeAttackTypes.ts';

export const getTeam = (coop: boolean, players: TimeAttackPlayer[]) => {
  return coop ? players.map((p) => p.units[0]) : players[0].units;
};

export const getImagePath = (relativePath: string) => new URL(relativePath, PUBLIC_CDN_URL).href;
