<script lang="ts">
	import { ModeWatcher } from 'mode-watcher';

  import Header from './header.svelte';
  import SideNav from './sideNav.svelte';
  import type { LayoutData } from './$types';
  import { onMount } from 'svelte';

  export let data: LayoutData;

  onMount(() => {
    console.log(data);
  });
</script>

<ModeWatcher />
<Header userInfo={data.userInfo} />
<SideNav />

<main class="pl-0 md:pl-[var(--navigation-width)]" style:padding-top="var(--header-height)">
	<slot />
</main>
