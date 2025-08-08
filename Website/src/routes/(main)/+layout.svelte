<script lang="ts">
  import { ModeWatcher } from 'mode-watcher';

  import { Toaster } from '$shadcn/components/ui/sonner';

  import type { LayoutProps } from './$types';
  import Header from './header.svelte';
  import SideNav from './sideNav.svelte';

  const { data, children }: LayoutProps = $props();
</script>

<svelte:head>
  <meta property="og:url" content={data.urlOrigin} />
</svelte:head>

<ModeWatcher disableHeadScriptInjection />
<Header hasValidJwt={data.hasValidJwt} isAdmin={data.isAdmin} />
<SideNav hasValidJwt={data.hasValidJwt} isAdmin={data.isAdmin} />
<Toaster richColors />

<main class="pl-0 md:pl-[var(--navigation-width)]" style:padding-top="var(--header-height)">
  {@render children?.()}
</main>
