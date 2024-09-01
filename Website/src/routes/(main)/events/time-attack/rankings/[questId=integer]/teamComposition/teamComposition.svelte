<script lang="ts">
  import type { TimeAttackUnit } from '../../timeAttackTypes.ts';
  import AbilityCrestIcon from './abilityCrestIcon.svelte';
  import CharaIcon from './charaIcon.svelte';
  import DragonIcon from './dragonIcon.svelte';
  import SkillIcon from './skillIcon.svelte';
  import TalismanIcon from './talismanIcon.svelte';
  import WeaponIcon from './weaponIcon.svelte';

  export let units: TimeAttackUnit[];
  export let unitKeys: string[] = ['Player 1', 'Player 2', 'Player 3', 'Player 4'];

  const spacerClass = 'max-w-0 flex-grow sm:max-w-0.5 md:max-w-2';
</script>

<h3 class="font-md mb-2 font-semibold">Team composition</h3>

<div class="flex flex-col gap-3">
  {#each units.map((unit, i) => ({ unit, key: unitKeys[i] })) as { unit, key }}
    <div>
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
          {#each unit.crests.slice(0, -2) as abilityCrest}
            <AbilityCrestIcon {abilityCrest} />
          {/each}
        </div>
        <div class="flex items-start">
          {#each unit.crests.slice(-2) as sindomAbilityCrest}
            <AbilityCrestIcon abilityCrest={sindomAbilityCrest} />
          {/each}
        </div>
        <span class={spacerClass} />
        <div class="flex items-start">
          {#each unit.sharedSkills as skill}
            <SkillIcon {skill} />
          {/each}
        </div>
      </div>
    </div>
  {/each}
</div>

<style>
  .spacer {
    flex-grow: 1;
  }

  #unit div {
    height: 45px;
  }
</style>
