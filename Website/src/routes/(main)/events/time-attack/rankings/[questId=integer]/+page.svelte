<script lang="ts">
  import { page } from '$app/stores';
  import Page from '$lib/components/page.svelte';
  import Typography from '$lib/components/typography.svelte';
  import { t } from '$lib/translations';

  import type { PageData } from './$types';
  import DataTable from './dataTable.svelte';

  export let data: PageData;

  $: currentQuest =
    data.questList.find((q) => q.id === parseInt($page.params.questId)) ?? data.questList[0];
</script>

<Page title="Time Attack Rankings">
  <div class="flex w-full flex-wrap items-start justify-between gap-4">
    <div>
      <p class="mb-2">Select a quest to view rankings:</p>
      <ul class="pl-4">
        {#each data.questList as { id: questId }}
          <li>
            <a
              class="hover:underline"
              aria-current={questId === currentQuest.id ? 'page' : undefined}
              href="/events/time-attack/rankings/{questId}">
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
  <Typography typography="h2">Clears</Typography>
  <DataTable data={data.clearData} coop={currentQuest.isCoop} />
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
</style>
