<script lang="ts">
  import { ModeWatcher } from 'mode-watcher';

  import type { LayoutData } from './$types';
  import Header from './header.svelte';
  import SideNav from './sideNav.svelte';
  import { Toaster } from '$shadcn/components/ui/sonner';

  export let data: LayoutData;
</script>

<svelte:head>
  <meta property="og:url" content={data.urlOrigin} />
</svelte:head>

<ModeWatcher />
<Header hasValidJwt={data.hasValidJwt} />
<SideNav hasValidJwt={data.hasValidJwt} />
<Toaster richColors />

<main class="pl-0 md:pl-[var(--navigation-width)]" style:padding-top="var(--header-height)">
  <slot />
</main>
