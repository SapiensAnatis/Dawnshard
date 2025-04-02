<script lang="ts">
  import { onMount } from 'svelte';

  import { navigating, page } from '$app/state';
  import Page from '$lib/components/page.svelte';

  import type { PageProps } from './$types';
  import NewsItem from './item.svelte';
  import { getPageNoFromParams, lastReadKey, pageSize } from './news.ts';
  import NewsPagination from './pagination.svelte';
  import NewsSkeleton from './skeleton.svelte';

  let { data }: PageProps = $props();

  let lastRead: Date = $state(new Date());

  let loading = $derived(navigating?.to && navigating.to.url.pathname.startsWith('/news'));
  let currentPage = $derived(getPageNoFromParams(page.url.searchParams));

  onMount(() => {
    const lastReadStorageItem = localStorage.getItem(lastReadKey);
    if (lastReadStorageItem) {
      lastRead = new Date(lastReadStorageItem);
    }

    localStorage.setItem(lastReadKey, new Date().toISOString());
  });
</script>

<Page title="News">
  <div class="mb-4 flex flex-col gap-3 px-3">
    {#if loading}
      {#each { length: pageSize } as _, index (index)}
        <NewsSkeleton />
      {/each}
    {:else}
      {#each data.newsItems.data as item (item.id)}
        <NewsItem {item} {lastRead} />
      {/each}
    {/if}
    <NewsPagination
      {currentPage}
      numPages={Math.ceil(data.newsItems.pagination.totalCount / pageSize)} />
  </div>
</Page>
