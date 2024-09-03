<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';

  export let weapon: TimeAttackUnit['weapon'];

  import { t } from '$lib/translations';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  type Weapon = TimeAttackUnit['weapon'];

  const getWeaponImagePath = (weapon: Weapon) => {
    const { baseId, formId } = weapon;
    const variationId = weapon.variationId.toString().padStart(2, '0');
    return new URL(`images/icon/weapon/m/${baseId}_${variationId}_${formId}.webp`, PUBLIC_CDN_URL)
      .href;
  };

  const weaponName = $t(`entity.weaponBody.item.${weapon.id}`);
</script>

<Popover.Root>
  <Popover.Trigger aria-label="Expand weapon details">
    <Image
      src={getWeaponImagePath(weapon)}
      layout="constrained"
      width={45}
      height={45}
      alt={weaponName} />
  </Popover.Trigger>
  <Popover.Content
    transitionConfig={{ y: -8, duration: 150 }}
    class="flex h-fit w-fit flex-col items-center pb-3 pt-2"
    side="bottom">
    {weaponName}
    <WikiLink pageName={weaponName} />
  </Popover.Content>
</Popover.Root>
