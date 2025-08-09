<script lang="ts">
  import ChevronLeft from 'lucide-svelte/icons/chevron-left';
  import ChevronRight from 'lucide-svelte/icons/chevron-right';
  import { SvelteURLSearchParams } from 'svelte/reactivity';

  import { page } from '$app/stores';
  import { Button } from '$shadcn/components/ui/button';

  export let currentPage: number;
  export let numPages: number;

  const getPagePath = (pageNo: number) => {
    const params = new SvelteURLSearchParams($page.url.searchParams);
    params.set('page', pageNo.toString());

    return `?${params}`;
  };

  $: previousPagePath = getPagePath(currentPage - 1);
  $: nextPagePath = getPagePath(currentPage + 1);
</script>

<div class="flex flex-col">
  <div class="grid w-[20rem] grid-cols-5 items-center gap-3 self-center">
    {#if currentPage > 1}
      <Button variant="ghost" href={previousPagePath} class="col-span-2">
        <ChevronLeft size="16" class="mr-2" aria-hidden="true" />
        Previous
      </Button>
    {:else}
      <span class="col-span-2" aria-hidden="true"></span>
    {/if}
    <p class="col-span-1 text-center text-sm">
      {currentPage} / {numPages}
    </p>
    {#if currentPage < numPages}
      <Button variant="ghost" href={nextPagePath} class="col-span-2">
        Next
        <ChevronRight size="16" class="ml-2" aria-hidden="true" />
      </Button>
    {:else}
      <span class="grid-cols-2" aria-hidden="true"></span>
    {/if}
  </div>
</div>
