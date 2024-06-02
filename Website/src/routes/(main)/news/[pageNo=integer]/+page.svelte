<script lang="ts">
  import type { PageData } from './$types';
  import { lastReadKey, pageSize } from '../news.ts';
  import { page, navigating } from '$app/stores';
  import NewsPagination from '../pagination.svelte';
  import NewsSkeleton from '../skeleton.svelte';
  import NewsItem from '../item.svelte';
  import { onMount } from 'svelte';

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

<div class="px-6 pt-4">
  <h1 class="text-4xl font-bold tracking-tight">News</h1>
  <hr class="mb-1 mt-2" />
  <div class="flex flex-col gap-3 p-3">
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
</div>
