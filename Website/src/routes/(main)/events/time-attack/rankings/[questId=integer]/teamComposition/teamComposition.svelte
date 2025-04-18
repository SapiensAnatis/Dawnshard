<script lang="ts">
  import type { TimeAttackUnit } from '../../timeAttackTypes.ts';
  import AbilityCrestIcon from './abilityCrestIcon.svelte';
  import CharaIcon from './charaIcon.svelte';
  import DragonIcon from './dragonIcon.svelte';
  import SkillIcon from './skillIcon.svelte';
  import TalismanIcon from './talismanIcon.svelte';
  import WeaponIcon from './weaponIcon.svelte';

  let {
    units,
    unitKeys,
    coop,
    key
  }: { units: TimeAttackUnit[]; unitKeys: string[]; coop: boolean; key: unknown } = $props();

  const id = `team-composition-${key}`;

  const spacerClass = 'max-w-0 flex-grow sm:max-w-0.5 md:max-w-2';
</script>

<h3 class="font-md mb-2 font-semibold" id={`${id}-header`}>Team composition</h3>

<ul class="flex flex-col gap-3" aria-labelledby={`${id}-header`}>
  {#each units.map((unit, i) => ({ unit, key: unitKeys[i] })) as { unit, key } (key)}
    <li aria-label={key}>
      <p class="text-muted-foreground mb-1">{key}</p>
      <div id="unit" class="flex flex-wrap">
        <div>
          <CharaIcon chara={unit.chara} />
        </div>
        <span class={spacerClass}></span>
        <div class="flex items-start">
          <DragonIcon dragon={unit.dragon} />
          <WeaponIcon weapon={unit.weapon} />
          <TalismanIcon talisman={unit.talisman} />
        </div>
        <span class={spacerClass}></span>
        <div class="flex items-start">
          {#each unit.crests.slice(0, 3) as abilityCrest (abilityCrest?.id)}
            <AbilityCrestIcon {abilityCrest} rarity={5} />
          {/each}
          {#each unit.crests.slice(3, -2) as abilityCrest (abilityCrest?.id)}
            <AbilityCrestIcon {abilityCrest} rarity={4} />
          {/each}
        </div>
        <div class="flex items-start">
          {#each unit.crests.slice(-2) as sindomAbilityCrest (sindomAbilityCrest?.id)}
            <AbilityCrestIcon abilityCrest={sindomAbilityCrest} rarity={6} />
          {/each}
        </div>
        <span class={spacerClass}></span>
        {#if unit.position === 1 || coop}
          <div class="flex items-start">
            {#each unit.sharedSkills as skill (skill?.id)}
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
