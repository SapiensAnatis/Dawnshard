<script lang="ts">
  import type { TimeAttackUnit } from '../../timeAttackTypes.ts';
  import AbilityCrestIcon from './abilityCrestIcon.svelte';
  import CharaIcon from './charaIcon.svelte';
  import DragonIcon from './dragonIcon.svelte';
  import SkillIcon from './skillIcon.svelte';
  import TalismanIcon from './talismanIcon.svelte';
  import WeaponIcon from './weaponIcon.svelte';

  export let units: TimeAttackUnit[];
</script>

<h3 class="font-md mb-2 font-semibold">Team composition</h3>

<div class="flex flex-col gap-1">
  {#each units as unit}
    <div class="grid max-w-[850px] grid-cols-12 grid-rows-1">
      <div class="col-span-2 md:col-span-1">
        <CharaIcon chara={unit.chara} />
      </div>
      <div class="col-span-10 flex items-start sm:col-span-3 md:col-span-3">
        <DragonIcon dragon={unit.dragon} />
        <WeaponIcon weapon={unit.weapon} />
        <TalismanIcon talisman={unit.talisman} />
      </div>
      <div class="col-span-12 flex items-start sm:col-span-4 md:col-span-6 lg:col-span-5">
        {#each unit.crests as abilityCrest}
          <AbilityCrestIcon {abilityCrest} />
        {/each}
      </div>
      <div class="col-span-6 sm:col-span-3 md:col-span-2">
        {#each unit.sharedSkills as skill}
          <SkillIcon {skill} />
        {/each}
      </div>
    </div>
  {/each}
</div>
