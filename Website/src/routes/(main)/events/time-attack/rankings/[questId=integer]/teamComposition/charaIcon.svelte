<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';

  export let chara: TimeAttackUnit['chara'];

  import { t } from '$lib/translations';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  const getCharaImagePath = (chara: TimeAttackUnit['chara']) => {
    const baseId = chara.baseId;
    const variationId = chara.variationId.toString().padStart(2, '0');
    return new URL(`images/icon/chara/m/${baseId}_${variationId}_r05.webp`, PUBLIC_CDN_URL).href;
  };

  const charaName = $t(`entity.chara.item.${chara.id}`);
  const charaImagePath = getCharaImagePath(chara);
</script>

<Popover.Root>
  <Popover.Trigger aria-label="Expand character details">
    <Image src={charaImagePath} layout="constrained" width={45} height={45} alt={charaName} />
  </Popover.Trigger>
  <Popover.Content
    transitionConfig={{ y: -8, duration: 150 }}
    class="flex h-fit w-fit flex-col items-center pb-3 pt-2"
    side="bottom">
    {charaName}
    <WikiLink pageName={charaName} />
  </Popover.Content>
</Popover.Root>
