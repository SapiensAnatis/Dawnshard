<script lang="ts">
  import type { TimeAttackUnit } from '../timeAttackTypes.ts';
  import CharaIcon from './teamComposition/charaIcon.svelte';

  let { team }: { team: TimeAttackUnit[] } = $props();
</script>

<div class="flex w-full items-center justify-between">
  <div class="flex gap-0">
    {#each team as teamUnit (teamUnit.position)}
      <CharaIcon chara={teamUnit.chara} />
    {/each}
  </div>
</div>
