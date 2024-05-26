<script lang="ts">
  import { Card, CardContent } from '$shadcn/components/ui/card/index.js';
  import { Image } from '@unpic/svelte';
  import type { NewsItem } from '$main/news/news.ts';
  import Header from './header.svelte';
  import { Newspaper } from 'lucide-svelte';

  export let item: NewsItem;
  export let lastRead: Date;
  export let description: boolean = true;
</script>

<Card
  class="grid grid-flow-row grid-cols-1 justify-start gap-0 overflow-hidden lg:grid-flow-col lg:grid-cols-none lg:grid-rows-[min-content_min-content]">
  <div
    class="flex h-full w-full items-center justify-center md:row-span-1 md:h-[200px] lg:row-span-2 lg:h-full lg:w-[13rem]">
    {#if item.headerImageSrc}
      <Image src={item.headerImageSrc} layout="fullWidth" height={200} />
    {:else}
      <Newspaper class="h-[10rem] w-[10rem] p-4" strokeWidth={1} />
    {/if}
  </div>
  <Header {item} {lastRead} />
  {#if description}
    <CardContent class="">
      <!-- Trusted input from API server - XSS is unlikely without server being compromised -->
      <!-- eslint-disable-next-line svelte/no-at-html-tags -->
      {@html item.description}
    </CardContent>
  {/if}
</Card>
