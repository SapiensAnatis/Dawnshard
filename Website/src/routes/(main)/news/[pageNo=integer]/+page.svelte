<script lang="ts">
  import { onMount } from 'svelte';

  import { navigating, page } from '$app/stores';
  import Page from '$lib/components/page.svelte';

  import NewsItem from '../item.svelte';
  import { lastReadKey, pageSize } from '../news.ts';
  import NewsPagination from '../pagination.svelte';
  import NewsSkeleton from '../skeleton.svelte';
  import type { PageData } from './$types';

  export let data: PageData;

  let lastRead: Date;

  $: loading = $navigating?.to && $navigating.to.url.pathname.startsWith('/news');
  $: currentPage = Number.parseInt($page.params.pageNo) || 1;

  onMount(() => {
    const lastReadStorageItem = localStorage.getItem(lastReadKey);
    if (lastReadStorageItem) {
      lastRead = new Date(lastReadStorageItem);
    } else {
      lastRead = new Date();
    }

    localStorage.setItem(lastReadKey, new Date().toISOString());
  });
</script>

<Page title="News">
  <div class="mb-4 flex flex-col gap-3 px-3">
    {#if loading}
      {#each { length: pageSize } as _}
        <NewsSkeleton />
      {/each}
    {:else}
      {#each data.newsItems.data as item}
        <NewsItem {item} {lastRead} />
      {/each}
    {/if}
    <NewsPagination
      {currentPage}
      numPages={Math.ceil(data.newsItems.pagination.totalCount / pageSize)} />
  </div>
</Page>
