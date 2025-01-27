<script lang="ts">
  import { Image } from '@unpic/svelte';
  import { Newspaper } from 'lucide-svelte';

  import { Card, CardContent } from '$shadcn/components/ui/card/index.js';

  import Header from './header.svelte';
  import { getImageSrc, type NewsItem } from './news.ts';

  const {
    item,
    lastRead,
    description = true
  }: { item: NewsItem; lastRead: Date; description: boolean } = $props();
  const headerImageSrc = $derived(getImageSrc(item.headerImagePath));
</script>

<Card class="flex flex-col overflow-hidden lg:flex-row">
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
  <div class:pb-7={!description}>
    <Header {item} {lastRead} />
    {#if description}
      <CardContent class="min-h-32">
        <!-- Trusted input from API server - XSS is unlikely without server being compromised -->
        <!-- eslint-disable-next-line svelte/no-at-html-tags -->
        {@html item.description}
      </CardContent>
    {/if}
  </div>
</Card>
