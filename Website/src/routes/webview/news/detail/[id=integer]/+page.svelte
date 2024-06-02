<script lang="ts">
  import { Image } from '@unpic/svelte';
  import type { PageData } from './$types';
  import { afterNavigate } from '$app/navigation';
  import { Button } from '$shadcn/components/ui/button';
  import ChevronLeft from 'lucide-svelte/icons/chevron-left';

  let previousPage: string = '/webview/news/1';

  afterNavigate(({ from }) => {
    previousPage = from?.url.pathname || previousPage;
  });

  export let data: PageData;

  let item = data.newsItem;
</script>

<div class="p-4">
  <Button variant="outline" href={previousPage}>
    <ChevronLeft size="16" class="mr-2" />
    Back to news
  </Button>
  <div class="mt-4">
    {#if item.headerImageSrc}
      <div class="mt-4 flex flex-row items-center justify-center">
        <Image class="rounded-md" src={item.headerImageSrc} layout="fullWidth" height={200} />
      </div>
      <br />
    {/if}
    <div>
      <h1 class="text-2xl font-semibold">
        {item.headline}
      </h1>
      <p class="text-sm text-gray-500">
        {item.date.toLocaleString(undefined, {
          dateStyle: 'long',
          timeStyle: 'short'
        })}
      </p>
      <br />
      <!-- Trusted input from API server - XSS is unlikely without server being compromised -->
      <!-- eslint-disable-next-line svelte/no-at-html-tags -->
      <p>{@html item.description}</p>
      <br />
      {#if item.bodyImageSrc}
        <Image src={item.bodyImageSrc} layout="fullWidth" class="w-full" />
      {/if}
    </div>
  </div>
</div>
