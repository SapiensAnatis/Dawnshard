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

<Card class="flex flex-col overflow-hidden lg:flex-row">
  <div class="flex shrink-0 grow-0 basis-52 items-center justify-center">
    {#if item.headerImageSrc}
      <Image src={item.headerImageSrc} class="hidden h-full lg:block" layout="fullWidth" />
      <Image src={item.headerImageSrc} height={208} class="block lg:hidden" layout="fullWidth" />
    {:else}
      <Newspaper class="size-[12rem] p-4" strokeWidth={1} />
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
