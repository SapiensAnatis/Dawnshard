<script lang="ts">
  import ChevronLeft from '@lucide/svelte/icons/chevron-left';
  import { Image } from '@unpic/svelte';

  import { afterNavigate } from '$app/navigation';
  import { formatDescription, getImageSrc } from '$main/news/news.ts';
  import { Button } from '$shadcn/components/ui/button';

  import type { PageProps } from './$types';

  let previousPage: string = $state('/webview/news');

  afterNavigate(({ from }) => {
    previousPage = from ? from.url.pathname + from.url.search : previousPage;
  });

  let { data }: PageProps = $props();

  let item = $derived(data.newsItem);

  let headerImageSrc = $derived(getImageSrc(item.headerImagePath));
  let bodyImageSrc = $derived(getImageSrc(item.bodyImagePath));
  let description = $derived(formatDescription(item.description));
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
      <p>{@html description}</p>
      <br />
      {#if bodyImageSrc}
        <Image src={bodyImageSrc} layout="fullWidth" class="w-full" alt={item.bodyImageAltText} />
      {/if}
    </div>
  </div>
</div>
