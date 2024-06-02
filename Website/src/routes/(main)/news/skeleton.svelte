<script lang="ts">
  import { Skeleton } from '$shadcn/components/ui/skeleton/index.js';
  import { Card, CardContent, CardHeader } from '$shadcn/components/ui/card/index.js';

  export let description: boolean = true;
</script>

<Card
  class="grid grid-flow-row grid-cols-1 justify-start gap-0 overflow-hidden lg:grid-cols-[13rem_1fr] lg:grid-rows-[min-content_min-content]">
  <div
    class="flex h-full w-full items-center justify-center md:row-span-1 md:h-[200px] lg:row-span-2 lg:h-full lg:w-[13rem]">
    <Skeleton class="h-[200px] w-full p-4" />
  </div>
  <CardHeader class="">
    <!-- Todo update for image -->
    <Skeleton class="h-6 w-[75%] max-w-[20rem]" />
    <Skeleton class="h-4 w-[35%] max-w-[10rem]" />
  </CardHeader>
  {#if description}
    <CardContent>
      <Skeleton class="mb-1 h-4 w-[95%]"></Skeleton>
      <Skeleton class="h-4 w-[40%]"></Skeleton>
    </CardContent>
  {/if}
</Card>
