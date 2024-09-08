<script lang="ts">
  import { page } from '$app/stores';
  import { Button } from '$shadcn/components/ui/button';

  export let hasValidJwt: boolean;
</script>

<h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
<div class="flex-grow" />

{#if hasValidJwt}
  <Button href="/logout" variant="secondary" data-sveltekit-reload>Log out</Button>
{:else}
  <Button href={`/login?originalPage=${$page.url.pathname}`}>Login</Button>
{/if}
