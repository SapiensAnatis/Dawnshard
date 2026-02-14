<script lang="ts">
  import { type NewsItem } from '$main/news/news.ts';
  import { Badge } from '$shadcn/components/ui/badge/index.js';
  import { CardDescription, CardHeader, CardTitle } from '$shadcn/components/ui/card/index.js';

  export let item: NewsItem;
  export let lastRead: Date;
</script>

<CardHeader class="">
  <div class="flex items-center gap-4">
    <CardTitle class="text-2xl">
      <h2 class="text-2xl">{item.headline}</h2>
    </CardTitle>
    {#if item.date > lastRead}
      <Badge class="h-6">New!</Badge>
    {/if}
  </div>
  <CardDescription class="mb-1">
    {item.date.toLocaleString(undefined, {
      dateStyle: 'medium',
      timeStyle: 'short'
    })}
  </CardDescription>
</CardHeader>
