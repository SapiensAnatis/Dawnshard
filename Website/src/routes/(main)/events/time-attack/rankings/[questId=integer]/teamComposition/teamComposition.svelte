<script lang="ts">
  import type { TimeAttackUnit } from '../../timeAttackTypes.ts';
  import AbilityCrestIcon from './abilityCrestIcon.svelte';
  import CharaIcon from './charaIcon.svelte';
  import DragonIcon from './dragonIcon.svelte';
  import SkillIcon from './skillIcon.svelte';
  import TalismanIcon from './talismanIcon.svelte';
  import WeaponIcon from './weaponIcon.svelte';

  export let units: TimeAttackUnit[];
  export let unitKeys: string[];
  export let coop: boolean;
  export let key;

  const id = `team-composition-${key}`;

  const spacerClass = 'max-w-0 flex-grow sm:max-w-0.5 md:max-w-2';
</script>

<h3 class="font-md mb-2 font-semibold" id={`${id}-header`}>Team composition</h3>

<ul class="flex flex-col gap-3" aria-labelledby={`${id}-header`}>
  {#each units.map((unit, i) => ({ unit, key: unitKeys[i] })) as { unit, key }}
    <li aria-label={key}>
      <p class="mb-1 text-muted-foreground">{key}</p>
      <div id="unit" class="flex flex-wrap">
        <div>
          <CharaIcon chara={unit.chara} />
        </div>
        <span class={spacerClass} />
        <div class="flex items-start">
          <DragonIcon dragon={unit.dragon} />
          <WeaponIcon weapon={unit.weapon} />
          <TalismanIcon talisman={unit.talisman} />
        </div>
        <span class={spacerClass} />
        <div class="flex items-start">
          {#each unit.crests.slice(0, 3) as abilityCrest}
            <AbilityCrestIcon {abilityCrest} rarity={5} />
          {/each}
          {#each unit.crests.slice(3, -2) as abilityCrest}
            <AbilityCrestIcon {abilityCrest} rarity={4} />
          {/each}
        </div>
        <div class="flex items-start">
          {#each unit.crests.slice(-2) as sindomAbilityCrest}
            <AbilityCrestIcon abilityCrest={sindomAbilityCrest} rarity={6} />
          {/each}
        </div>
        <span class={spacerClass} />
        {#if unit.position === 1 || coop}
          <div class="flex items-start">
            {#each unit.sharedSkills as skill}
              <SkillIcon {skill} />
            {/each}
          </div>
        {/if}
      </div>
    </li>
  {/each}
</ul>

<style>
  #unit div {
    height: 45px;
  }
</style>
