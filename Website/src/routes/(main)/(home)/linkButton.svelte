<script lang="ts">
  import type { Component } from 'svelte';

  import { Button } from '$shadcn/components/ui/button/index.js';

  export let href: string;
  export let icon: Component;
</script>

<Button variant="secondary" {href}>
  <span aria-hidden="true" class="fill-accent-foreground mr-2">
    <svelte:component this={icon} />
  </span>
  <slot />
</Button>
