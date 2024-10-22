<script lang="ts">
  import type { Component } from 'svelte';

  import { Button } from '$shadcn/components/ui/button';

  export let href: string;
  export let icon: Component;
</script>

<Button variant="secondary" {href}>
  <span aria-hidden="true" class="fill-accent-foreground">
    <svelte:component this={icon} />
  </span>
</Button>
