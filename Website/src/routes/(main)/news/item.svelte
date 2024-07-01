<script lang="ts">
  import { Image } from '@unpic/svelte';
  import { Newspaper } from 'lucide-svelte';

  import type { NewsItem } from '$main/news/news.ts';
  import { Card, CardContent } from '$shadcn/components/ui/card/index.js';

  import Header from './header.svelte';

  export let item: NewsItem;
  export let lastRead: Date;
  export let description: boolean = true;
</script>

<Card
  class="flex flex-col overflow-hidden lg:grid lg:grid-flow-col lg:grid-cols-none lg:grid-rows-1 lg:justify-start">
  <div class="flex items-center justify-center md:h-[200px] lg:h-full lg:w-[13rem]">
    {#if item.headerImageSrc}
      <Image src={item.headerImageSrc} layout="fullWidth" class="hidden h-[100%] lg:block" />
      <Image src={item.headerImageSrc} height={200} class="block lg:hidden" layout="fullWidth" />
    {:else}
      <Newspaper class="h-[12rem] w-[12rem] p-4" strokeWidth={1} />
    {/if}
  </div>
  <div>
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
