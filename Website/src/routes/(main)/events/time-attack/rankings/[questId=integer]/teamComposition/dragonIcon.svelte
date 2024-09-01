<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';

  export let dragon: TimeAttackUnit['dragon'];

  import { t } from '$lib/translations';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  type Dragon = NonNullable<TimeAttackUnit['dragon']>;

  const getDragonImagePath = (dragon: Dragon) => {
    const baseId = dragon.baseId;
    const variationId = dragon.variationId.toString().padStart(2, '0');
    return new URL(`images/icon/dragon/m/${baseId}_${variationId}.webp`, PUBLIC_CDN_URL).href;
  };

  const getDragonName = (dragon: Dragon) => $t(`entity.dragon.item.${dragon.id}`);
</script>

{#if dragon}
  <Popover.Root>
    <Popover.Trigger aria-label="Expand dragon details">
      <Image
        src={getDragonImagePath(dragon)}
        layout="constrained"
        width={45}
        height={45}
        alt={getDragonName(dragon)} />
    </Popover.Trigger>
    <Popover.Content
      transitionConfig={{ y: -8, duration: 150 }}
      class="flex h-fit w-fit flex-col items-center pb-3 pt-2"
      side="bottom">
      {getDragonName(dragon)}
      <WikiLink pageName={getDragonName(dragon)} />
    </Popover.Content>
  </Popover.Root>
{:else}
  <Image
    src={new URL(`images/icon/others/Icon_Blank_04.webp`, PUBLIC_CDN_URL).href}
    alt="Empty dragon slot"
    layout="constrained"
    width={45}
    height={45} />
{/if}
