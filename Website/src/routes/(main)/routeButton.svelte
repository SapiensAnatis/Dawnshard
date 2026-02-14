<script lang="ts">
  import type { Route } from '$main/routes.ts';
  import { Button } from '$shadcn/components/ui/button/index.js';

  let { route }: { route: Route } = $props();

  let Icon = $derived(route.icon);
</script>

<Button href={route.href} variant="ghost" class="flex h-11 w-full justify-start">
  <Icon class="mr-2 size-6 shrink-0 basis-6" aria-hidden="true" />
  <span>{route.title}</span>
</Button>
