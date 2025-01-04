<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';

  export let abilityCrest: TimeAttackUnit['crests'][0];
  export let rarity: number;

  import { t } from '$lib/translations';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  type AbilityCrest = NonNullable<TimeAttackUnit['crests'][0]>;

  const getAbilityCrestImagePath = (abilityCrest: AbilityCrest) => {
    const baseId = abilityCrest.baseId;
    const imageNum = abilityCrest.imageNum.toString().padStart(2, '0');
    return new URL(`images/icon/amulet/m/${baseId}_${imageNum}.webp`, PUBLIC_CDN_URL).href;
  };

  const getAbilityCrestName = (abilityCrest: AbilityCrest) =>
    $t(`entity.abilityCrest.item.${abilityCrest.id}`);

  const emptyIconRarityLookup: Partial<Record<number, string>> = {
    5: 'Icon_Blank_07_A.webp',
    4: 'Icon_Blank_07_B.webp',
    6: 'Icon_Blank_07_C.webp' // sindom
  };
</script>

{#if abilityCrest}
  <Popover.Root>
    <Popover.Trigger aria-label="Expand wyrmprint details">
      <Image
        src={getAbilityCrestImagePath(abilityCrest)}
        layout="constrained"
        width={45}
        height={45}
        alt={getAbilityCrestName(abilityCrest)} />
    </Popover.Trigger>
    <Popover.Content class="flex h-fit w-fit flex-col items-center pb-3 pt-2" side="bottom">
      {getAbilityCrestName(abilityCrest)}
      <WikiLink pageName={getAbilityCrestName(abilityCrest)} />
    </Popover.Content>
  </Popover.Root>
{:else}
  <Image
    src={new URL(`images/icon/others/${emptyIconRarityLookup[rarity]}`, PUBLIC_CDN_URL).href}
    alt="Empty wyrmprint slot"
    layout="constrained"
    width={45}
    height={45} />
{/if}
