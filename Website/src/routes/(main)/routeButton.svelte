<script lang="ts">
  import type { Route } from '$main/routes.ts';
  import { Button } from '$shadcn/components/ui/button/index.js';

  export let route: Route;
</script>

<Button href={route.href} variant="ghost" class="flex h-11  justify-start">
  <svelte:component this={route.icon} class="mr-2 size-6 shrink-0 basis-6" aria-hidden="true" />
  <span>{route.title}</span>
</Button>
