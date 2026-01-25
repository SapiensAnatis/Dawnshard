<script lang="ts">
  import { resolve } from '$app/paths';
  import Page from '$lib/components/page.svelte';
  import Typography from '$lib/components/typography.svelte';
  import { t } from '$lib/translations';

  import type { PageProps } from './$types';
  import DataTable from './dataTable.svelte';

  let { data, params }: PageProps = $props();

  let currentQuest = $derived(data.questList.find((q) => q.id === parseInt(params.questId)));
</script>

<Page title="Time Attack Rankings">
  <div class="mb-4 flex w-full flex-wrap items-start justify-between gap-4">
    <div>
      <p class="mb-2">Select a quest to view rankings:</p>
      <ul class="pl-4">
        {#each data.questList as { id: questId } (questId)}
          <li>
            <a
              class="hover:underline"
              aria-current={questId === currentQuest?.id ? 'page' : undefined}
              href={resolve('/(main)/events/time-attack/rankings/[questId=integer]', {
                questId: questId.toString()
              })}>
              {$t(`timeAttack.quest.${questId}`)}
            </a>
          </li>
        {/each}
      </ul>
    </div>
    <enhanced:img
      class="rounded-2xl border"
      src="$lib/assets/timeAttack/volk.png"
      alt="Promotional banner for selected time attack quest" />
  </div>
  <Typography typography="h2" id="time-attack-table-title">Clears</Typography>
  <DataTable
    data={data.clearData.data}
    itemCount={data.clearData.pagination.totalCount}
    coop={currentQuest?.isCoop ?? false} />
</Page>

<style>
  a:not([aria-current='page']) {
    color: hsl(var(--muted-foreground));
  }

  a[aria-current='page'] {
    font-weight: 600;
  }

  li + li {
    margin-top: 0.4rem;
  }

  :global(#time-attack-table-title) {
    scroll-margin-top: var(--header-height);
  }
</style>
