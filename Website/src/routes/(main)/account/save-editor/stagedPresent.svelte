<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';

  import type { PresentFormSubmission } from './presentTypes.ts';
  export let present: PresentFormSubmission;
</script>

<li class="present">
  <Gift aria-hidden={true} />
  <p>Add {present.quantity}x <strong>{present.itemLabel}</strong> to the gift box</p>
</li>

<style>
  .present {
    background: hsl(var(--background-surface));
    border-radius: 10px;
    padding: 1rem;
    display: flex;
    gap: 1rem;
    align-items: center;
  }
</style>
