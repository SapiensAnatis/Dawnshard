<script lang="ts">
  import { Newspaper } from '@lucide/svelte';
  import { Image } from '@unpic/svelte';

  import * as Card from '$shadcn/components/ui/card/index.js';

  import Header from './header.svelte';
  import { formatDescription, getImageSrc, type NewsItem } from './news.ts';

  const {
    item,
    lastRead,
    description = true
  }: { item: NewsItem; lastRead: Date; description?: boolean } = $props();

  const headerImageSrc = $derived(getImageSrc(item.headerImagePath));
  const formattedDescription = $derived(formatDescription(item.description));
</script>

<Card.Root class="flex flex-col overflow-hidden py-0 lg:flex-row">
  <div class="flex shrink-0 grow-0 basis-52 items-center justify-center">
    {#if headerImageSrc}
      <Image
        src={headerImageSrc}
        class="h-[208px] w-full object-cover lg:h-full"
        layout="fullWidth"
        alt={item.headerImageAltText} />
    {:else}
      <Newspaper class="size-[12rem] p-4" strokeWidth={1} aria-label="Newspaper vector icon" />
    {/if}
  </div>
  <div class:pb-7={!description} class="py-4">
    <Header {item} {lastRead} />
    {#if description}
      <Card.Content class="min-h-32">
        <!--
          Trusted input from API server - XSS is unlikely without server being compromised, and our
          Content-Security-Policy should block inline scripts anyway
         -->
        <!-- eslint-disable-next-line svelte/no-at-html-tags -->
        {@html formattedDescription}
      </Card.Content>
    {/if}
  </div>
</Card.Root>
