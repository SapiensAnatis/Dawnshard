<script lang="ts">
  import { onMount } from 'svelte';

  import { goto } from '$app/navigation';
  import { page } from '$app/stores';
  import BuyMeACoffee from '$main/(home)/icons/buyMeACoffee.svelte';
  import Discord from '$main/(home)/icons/discord.svelte';
  import GitHub from '$main/(home)/icons/github.svelte';
  import Patreon from '$main/(home)/icons/patreon.svelte';
  import NewsItem from '$main/news/item.svelte';
  import { lastReadKey, pageSize } from '$main/news/news.ts';
  import NewsPagination from '$main/news/pagination.svelte';

  import IconButton from '../iconButton.svelte';
  import type { PageData } from './$types';

  let lastRead: Date;

  export let data: PageData;

  $: currentPage = Number.parseInt($page.params.pageNo) || 1;

  onMount(() => {
    // Required for showing the Mercurial Gauntlet rewards page, which unconditionally routes to
    // the URL we provide for the news page, plus #detail/20000 in the hash.
    // Hash-based routing is not really supported by SvelteKit, so we will just rewrite to an actual route.
    const openedNewsId = Number.parseInt($page.url.hash.replace('#detail/', '')) || null;

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
    goto(`/webview/news/detail/${id}`);
  };
</script>

<div class="px-6 pt-4">
  <div class="flex flex-col items-center gap-4">
    <h1 class="text-nowrap text-3xl font-bold tracking-tight">Welcome to Dawnshard</h1>
    <div class="flex flex-wrap gap-2">
      <IconButton href="https://github.com/sapiensanatis/dawnshard" icon={GitHub} />
      <IconButton href="https://discord.gg/j9zSttjjWj" icon={Discord} />
      <IconButton href="https://patreon.com/dawnshard" icon={Patreon} />
      <IconButton href="https://buymeacoffee.com/dawnshard" icon={BuyMeACoffee} />
    </div>
  </div>
  <hr class="mb-1 mt-4" />
  <div class="flex flex-col gap-3 p-3">
    {#each data.news.data as item}
      <a tabindex="0" href={`/webview/news/detail/${item.id}`}>
        <NewsItem {item} description={false} {lastRead} />
      </a>
    {/each}
    <NewsPagination
      {currentPage}
      numPages={Math.ceil(data.news.pagination.totalCount / pageSize)} />
  </div>
</div>
