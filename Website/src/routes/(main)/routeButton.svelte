<script lang="ts">
  import type { Route } from '$main/routes.ts';
  import { Button } from '$shadcn/components/ui/button/index.js';

  export let route: Route;
</script>

<Button href={route.href} variant="ghost" class="h-11 w-[90%] justify-start">
  <svelte:component this={route.icon} class="mr-2 size-6" aria-hidden="true" />
  {route.title}
</Button>
