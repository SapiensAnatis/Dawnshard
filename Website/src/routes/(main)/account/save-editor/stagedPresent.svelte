<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';

  import type { PresentFormSubmission } from './presentTypes.ts';
  export let present: PresentFormSubmission;
</script>

<div class="present">
  <Gift aria-hidden={true} />
  <p>Add {present.quantity}x <strong>{present.itemLabel}</strong> to the gift box</p>
</div>

<style>
  div {
    background: hsl(var(--background-surface));
    border-radius: 10px;
    padding: 1rem;
    display: flex;
    gap: 1rem;
    align-items: center;
  }
</style>
