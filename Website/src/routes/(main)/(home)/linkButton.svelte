<script lang="ts">
  import type { ComponentType } from 'svelte';
  import { Button } from '$shadcn/components/ui/button/index.js';

  export let href: string;
  export let icon: ComponentType;
</script>

<Button variant="secondary" {href}>
  <span aria-hidden="true" class="mr-2 fill-accent-foreground">
    <svelte:component this={icon} />
  </span>
  <slot />
</Button>
