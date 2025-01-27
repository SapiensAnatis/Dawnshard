<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';

  export let talisman: TimeAttackUnit['talisman'];

  import { t } from '$lib/translations';
  import * as Popover from '$shadcn/components/ui/popover';

  type Talisman = NonNullable<TimeAttackUnit['talisman']>;

  const getTalismanImagePath = (talisman: Talisman) =>
    new URL(`images/icon/talisman/m/${talisman.id}.webp`, PUBLIC_CDN_URL).href;

  const getTalismanName = (talisman: Talisman) => $t(`entity.talisman.item.${talisman.id}`);

  const getAbilityName = (abilityId: number, talisman: Talisman) => {
    const element = $t(`timeAttack.element.${talisman.element}`);
    const weaponType = $t(`timeAttack.weaponType.${talisman.weaponType}`);
    return $t(`timeAttack.ability.${abilityId}`, { element, weaponType });
  };
</script>

{#if talisman}
  <Popover.Root>
    <Popover.Trigger aria-label="Expand portrait wyrmprint details">
      <Image
        src={getTalismanImagePath(talisman)}
        layout="constrained"
        width={45}
        height={45}
        alt={getTalismanName(talisman)} />
    </Popover.Trigger>
    <Popover.Content class="flex h-fit w-fit flex-col items-center pt-2 pb-3" side="bottom">
      {getTalismanName(talisman)}
      <ol>
        <li class="text-sm">
          {#if talisman.ability1Id}
            {getAbilityName(talisman.ability1Id, talisman)}
          {:else}
            No ability
          {/if}
        </li>
        <li class="text-sm">
          {#if talisman.ability2Id}
            {getAbilityName(talisman.ability2Id, talisman)}
          {:else}
            No ability
          {/if}
        </li>
      </ol>
    </Popover.Content>
  </Popover.Root>
{:else}
  <Image
    src={new URL(`images/icon/others/Icon_Blank_07_D.webp`, PUBLIC_CDN_URL).href}
    alt="Empty portrait wyrmprint slot"
    layout="constrained"
    class=""
    width={45}
    height={45} />
{/if}

<style>
  li {
    list-style-type: decimal;
    list-style-position: inside;
  }
</style>
