<script lang="ts">
  import { onMount } from 'svelte';

  import { goto } from '$app/navigation';
  import { resolve } from '$app/paths';
  import { page } from '$app/state';
  import BuyMeACoffee from '$main/(home)/icons/buyMeACoffee.svelte';
  import Discord from '$main/(home)/icons/discord.svelte';
  import GitHub from '$main/(home)/icons/github.svelte';
  import Patreon from '$main/(home)/icons/patreon.svelte';
  import NewsItem from '$main/news/item.svelte';
  import { getPageNoFromParams, lastReadKey, pageSize } from '$main/news/news.ts';
  import NewsPagination from '$main/news/pagination.svelte';

  import type { PageProps } from './$types';
  import IconButton from './iconButton.svelte';

  let lastRead: Date = $state(new Date());

  let { data }: PageProps = $props();

  const currentPage = $derived(getPageNoFromParams(page.url.searchParams));

  onMount(() => {
    // Required for showing the Mercurial Gauntlet rewards page, which unconditionally routes to
    // the URL we provide for the news page, plus #detail/20000 in the hash.
    // Hash-based routing is not really supported by SvelteKit, so we will just rewrite to an actual route.
    const openedNewsId = Number.parseInt(page.url.hash.replace('#detail/', '')) || null;

    if (openedNewsId) {
      onOpenNewsItem(openedNewsId);
    }

    const lastReadStorageItem = localStorage.getItem(lastReadKey);
    if (lastReadStorageItem) {
      lastRead = new Date(lastReadStorageItem);
    } else {
      lastRead = new Date();
    }

    localStorage.setItem(lastReadKey, new Date().toISOString());
  });

  const onOpenNewsItem = (id: number) => {
    goto(resolve(`/webview/news/detail/[id=integer]`, { id: id.toString() }));
  };
</script>

<div class="px-6 pt-4">
  <div class="flex flex-col items-center gap-4">
    <h1 class="text-2xl font-bold tracking-tight text-nowrap">Welcome to Dawnshard</h1>
    <div class="flex flex-wrap gap-2">
      <IconButton href="https://github.com/sapiensanatis/dawnshard" icon={GitHub} />
      <IconButton href="https://discord.gg/j9zSttjjWj" icon={Discord} />
      <IconButton href="https://patreon.com/dawnshard" icon={Patreon} />
      <IconButton href="https://buymeacoffee.com/dawnshard" icon={BuyMeACoffee} />
    </div>
  </div>
  <hr class="mt-4 mb-1" />
  <div class="flex flex-col gap-3 p-3">
    {#each data.news.data as item (item.id)}
      <a
        tabindex="0"
        href={resolve('/webview/news/detail/[id=integer]', { id: item.id.toString() })}>
        <NewsItem {item} description={false} {lastRead} />
      </a>
    {/each}
    <NewsPagination
      {currentPage}
      numPages={Math.ceil(data.news.pagination.totalCount / pageSize)} />
  </div>
</div>
