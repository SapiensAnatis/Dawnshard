<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import { l, t } from '$lib/translations';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  const { chara }: { chara: TimeAttackUnit['chara'] } = $props();

  const getCharaImagePath = (chara: TimeAttackUnit['chara']) => {
    const baseId = chara.baseId;
    const variationId = chara.variationId.toString().padStart(2, '0');
    return new URL(`images/icon/chara/m/${baseId}_${variationId}_r05.webp`, PUBLIC_CDN_URL).href;
  };

  const charaNameKey = $derived(`entity.chara.item.${chara.id}`);
  const charaName = $derived($t(charaNameKey));
  const charaWikiName = $derived($l('en', charaNameKey));
  const charaImagePath = $derived(getCharaImagePath(chara));
</script>

<Popover.Root>
  <Popover.Trigger aria-label="Expand character details">
    <Image src={charaImagePath} layout="constrained" width={45} height={45} alt={charaName} />
  </Popover.Trigger>
  <Popover.Content class="flex h-fit w-fit flex-col items-center pt-2 pb-3" side="bottom">
    {charaName}
    <WikiLink pageName={charaWikiName} />
  </Popover.Content>
</Popover.Root>
