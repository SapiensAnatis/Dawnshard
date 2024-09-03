<script lang="ts">
  import ChevronDown from 'lucide-svelte/icons/chevron-down';
  import ChevronUp from 'lucide-svelte/icons/chevron-up';
  import type { Writable } from 'svelte/store';

  import { Button } from '$shadcn/components/ui/button';

  import type { TimeAttackUnit } from '../timeAttackTypes.ts';
  import CharaIcon from './teamComposition/charaIcon.svelte';

  export let team: TimeAttackUnit[];
  export let isExpanded: Writable<boolean>;
</script>

<div class="flex w-full items-center justify-between">
  <div class="flex gap-0">
    {#each team as teamUnit}
      <CharaIcon chara={teamUnit.chara} />
    {/each}
  </div>
  <Button
    variant="ghost"
    size="sm"
    class="rounded-full p-1 align-middle"
    aria-label="View detailed team information"
    aria-pressed={$isExpanded}
    on:click={() => ($isExpanded = !$isExpanded)}>
    {#if $isExpanded}
      <ChevronUp aria-hidden="true" />
    {:else}
      <ChevronDown aria-hidden="true" />
    {/if}
  </Button>
</div>
