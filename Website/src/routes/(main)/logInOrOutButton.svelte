<script lang="ts">
  import { page } from '$app/state';
  import { Button } from '$shadcn/components/ui/button';

  let { hasValidJwt }: { hasValidJwt: boolean } = $props();

  const getOriginalPage = () => {
    const searchParam = page.url.searchParams.get('originalPage');
    if (searchParam) {
      return searchParam; // already URL encoded
    }

    return encodeURIComponent(page.url.pathname);
  };
</script>

{#if hasValidJwt}
  <Button href="/logout" variant="destructive" data-sveltekit-reload>Log out</Button>
{:else}
  <Button href={`/login?originalPage=${getOriginalPage()}`} data-sveltekit-preload-data="off">
    Login
  </Button>
{/if}
