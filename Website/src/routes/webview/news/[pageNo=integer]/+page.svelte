<script lang="ts">
  import { onMount } from 'svelte';

  import { page } from '$app/stores';
  import type { PageData } from './$types';

  import { lastReadKey, pageSize } from '$main/news/news.ts';
  import NewsPagination from '$main/news/pagination.svelte';
  import NewsSkeleton from '$main/news/skeleton.svelte';
  import GitHub from '$main/(home)/icons/github.svelte';
  import Discord from '$main/(home)/icons/discord.svelte';
  import Patreon from '$main/(home)/icons/patreon.svelte';
  import BuyMeACoffee from '$main/(home)/icons/buyMeACoffee.svelte';
  import IconButton from '../iconButton.svelte';
  import { goto } from '$app/navigation';
  import NewsItem from '$main/news/item.svelte';

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

  // TODO: Reconsider dialog and maybe make all items route to a separate page with the hash
</script>

<div class="px-6 pt-4">
  <div class="flex flex-col items-center gap-4">
    <h1 class="text-3xl font-bold tracking-tight">Welcome to Dawnshard</h1>
    <div class="flex flex-wrap gap-2">
      <IconButton href="https://github.com/sapiensanatis/dawnshard" icon={GitHub} />
      <IconButton href="https://discord.gg/j9zSttjjWj" icon={Discord} />
      <IconButton href="https://patreon.com/dawnshard" icon={Patreon} />
      <IconButton href="https://buymeacoffee.com/dawnshard" icon={BuyMeACoffee} />
    </div>
  </div>
  <hr class="mb-1 mt-4" />
  <div class="flex flex-col gap-3 p-3">
    {#await data.newsPromise}
      {#each { length: pageSize } as _}
        <NewsSkeleton description={false} />
      {/each}
    {:then response}
      {#if !response}
        <p>Failed to load news!</p>
      {:else}
        {#each response.data as item}
          <a tabindex="0" href={`/webview/news/detail/${item.id}`}>
            <NewsItem {item} description={false} {lastRead} />
          </a>
        {/each}
        <NewsPagination
          {currentPage}
          numPages={Math.ceil(response.pagination.totalCount / pageSize)} />
      {/if}
    {/await}
  </div>
</div>
