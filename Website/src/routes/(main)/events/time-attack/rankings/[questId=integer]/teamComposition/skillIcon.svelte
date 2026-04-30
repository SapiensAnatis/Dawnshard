<script lang="ts">
  import { Image } from '@unpic/svelte';

  import { PUBLIC_CDN_URL } from '$env/static/public';
  import { l, t } from '$lib/translations';
  import type { TimeAttackUnit } from '$main/events/time-attack/rankings/timeAttackTypes.ts';
  import * as Popover from '$shadcn/components/ui/popover';

  import WikiLink from './wikiLink.svelte';

  type Skill = TimeAttackUnit['sharedSkills'][0];

  const { skill }: { skill: Skill } = $props();

  const getSkillImagePath = (skill: Skill) =>
    new URL(`images/icon/skill/m/${skill.skillLv4IconName}.webp`, PUBLIC_CDN_URL).href;

  const skillName = $derived($t(`timeAttack.skill.${skill.id}`));
  const skillWikiName = $derived($l('en', `timeAttack.skill.${skill.id}`));
</script>

<Popover.Root>
  <Popover.Trigger aria-label="Expand skill details">
    <Image
      src={getSkillImagePath(skill)}
      layout="constrained"
      width={45}
      height={45}
      alt={skillName} />
  </Popover.Trigger>
  <Popover.Content class="flex h-fit w-fit flex-col items-center pt-2 pb-3" side="bottom">
    {skillName}
    <WikiLink pageName={skillWikiName} />
  </Popover.Content>
</Popover.Root>
