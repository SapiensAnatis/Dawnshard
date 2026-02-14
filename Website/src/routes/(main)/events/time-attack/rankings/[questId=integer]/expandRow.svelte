<script lang="ts">
  import { ChevronDown, ChevronUp } from '@lucide/svelte';

  import { Button } from '$lib/shadcn/components/ui/button/index.js';

  let {
    initialized,
    isExpanded,
    toggleExpanded
  }: { initialized: boolean; isExpanded: boolean; toggleExpanded: () => void } = $props();
</script>

<Button
  variant="ghost"
  size="sm"
  class="rounded-full p-1 align-middle"
  aria-label="View detailed team information"
  aria-pressed={isExpanded}
  onclick={() => {
    toggleExpanded();
  }}
  disabled={!initialized}>
  {#if isExpanded}
    <ChevronUp aria-hidden="true" />
  {:else}
    <ChevronDown aria-hidden="true" />
  {/if}
</Button>
