<script lang="ts">
  import ChevronLeft from 'lucide-svelte/icons/chevron-left';
  import ChevronRight from 'lucide-svelte/icons/chevron-right';
  import { Button } from '$shadcn/components/ui/button';
  import { page } from '$app/stores';

  export let currentPage: number;
  export let numPages: number;

  $: previousPagePath = $page.url.pathname.replace(
    currentPage.toString(),
    (currentPage - 1).toString()
  );
  $: nextPagePath = $page.url.pathname.replace(
    currentPage.toString(),
    (currentPage + 1).toString()
  );

  const scrollToTop = () => window.scrollTo(0, 0);
</script>

<div class="flex flex-col">
  <div class="grid w-[20rem] grid-cols-5 items-center gap-3 self-center">
    {#if currentPage > 1}
      <Button variant="ghost" href={previousPagePath} class="col-span-2" on:click={scrollToTop}>
        <ChevronLeft size="16" class="mr-2" />
        Previous
      </Button>
    {:else}
      <span class="col-span-2" aria-hidden="true" />
    {/if}
    <p class="col-span-1 text-center text-sm">
      {currentPage} / {numPages}
    </p>
    {#if currentPage < numPages}
      <Button variant="ghost" href={nextPagePath} class="col-span-2" on:click={scrollToTop}>
        Next
        <ChevronRight size="16" class="ml-2" />
      </Button>
    {:else}
      <span class="grid-cols-2" aria-hidden="true" />
    {/if}
  </div>
</div>
