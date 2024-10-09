<script lang="ts">
  import { Image } from '@unpic/svelte';
  import ChevronLeft from 'lucide-svelte/icons/chevron-left';

  import { afterNavigate } from '$app/navigation';
  import { getImageSrc } from '$main/news/news.ts';
  import { Button } from '$shadcn/components/ui/button';

  import type { PageData } from './$types';

  let previousPage: string = '/webview/news';

  afterNavigate(({ from }) => {
    previousPage = from ? from.url.pathname + from.url.search : previousPage;
  });

  export let data: PageData;

  let item = data.newsItem;

  $: headerImageSrc = getImageSrc(item.headerImagePath);
  $: bodyImageSrc = getImageSrc(item.bodyImagePath);
</script>

<div class="p-4">
  <Button variant="outline" href={previousPage}>
    <ChevronLeft size="16" class="mr-2" />
    Back to news
  </Button>
  <div class="mt-4">
    {#if headerImageSrc}
      <div class="mt-4 flex flex-row items-center justify-center">
        <Image
          class="rounded-md"
          src={headerImageSrc}
          layout="fullWidth"
          height={200}
          alt={item.headerImageAltText} />
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
      {#if bodyImageSrc}
        <Image src={bodyImageSrc} layout="fullWidth" class="w-full" alt={item.bodyImageAltText} />
      {/if}
    </div>
  </div>
</div>
