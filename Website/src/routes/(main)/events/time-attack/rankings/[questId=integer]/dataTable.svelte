<script lang="ts">
  import {
    createColumnHelper,
    createTable,
    type ExpandedState,
    FlexRender,
    type PaginationState,
    renderComponent,
    rowExpandingFeature,
    rowPaginationFeature,
    tableFeatures
  } from '@tanstack/svelte-table';
  import { onMount, tick } from 'svelte';
  import { SvelteURLSearchParams } from 'svelte/reactivity';
  import { slide } from 'svelte/transition';

  import { goto } from '$app/navigation';
  import { page } from '$app/state';
  import {
    getTeam,
    getTeamKeys
  } from '$main/events/time-attack/rankings/[questId=integer]/util.ts';
  import type {
    TimeAttackPlayer,
    TimeAttackRanking
  } from '$main/events/time-attack/rankings/timeAttackTypes.ts';
  import { Button } from '$shadcn/components/ui/button';
  import * as Table from '$shadcn/components/ui/table';

  import ExpandRow from './expandRow.svelte';
  import TeamCell from './teamCell.svelte';
  import TeamComposition from './teamComposition/teamComposition.svelte';

  let { itemCount, data, coop }: { itemCount: number; data: TimeAttackRanking[]; coop: boolean } =
    $props();

  let pagination = $state<PaginationState>({ pageIndex: 0, pageSize: 10 });
  let expanded = $state<ExpandedState>({});

  const features = tableFeatures({ rowPaginationFeature, rowExpandingFeature });
  const columnHelper = createColumnHelper<typeof features, TimeAttackRanking>();

  const columns = columnHelper.columns([
    columnHelper.accessor('rank', {
      header: 'Rank'
    }),
    columnHelper.accessor('players', {
      header: () => (coop ? 'Players' : 'Player'),
      cell: (info) => {
        return info
          .getValue()
          .map((p: TimeAttackPlayer) => p.name)
          .join(', ');
      }
    }),
    columnHelper.accessor('time', {
      header: 'Clear Time',
      cell: (info) => {
        const date = new Date(info.getValue() * 1000);
        return date.toISOString().slice(14, -3);
      }
    }),
    columnHelper.accessor('players', {
      id: 'team',
      header: 'Team',
      cell: (info) => {
        return renderComponent(TeamCell, {
          team: getTeam(coop, info.getValue()),
          expanded: false
        });
      }
    }),
    columnHelper.display({
      id: 'expand',
      cell: ({ row }) => {
        return renderComponent(ExpandRow, {
          initialized,
          isExpanded: row.getIsExpanded(),
          toggleExpanded: () => row.toggleExpanded()
        });
      }
    })
  ]);

  const table = createTable({
    features,
    columns,
    get data() {
      return data;
    },
    state: {
      get pagination() {
        return pagination;
      },
      get expanded() {
        return expanded;
      }
    },
    onPaginationChange: (updaterOrValue) => {
      pagination =
        typeof updaterOrValue === 'function' ? updaterOrValue(pagination) : updaterOrValue;

      handlePageChange(pagination.pageIndex);
    },
    onExpandedChange: (updaterOrValue) => {
      expanded = typeof updaterOrValue === 'function' ? updaterOrValue(expanded) : updaterOrValue;
    },
    manualPagination: true,
    get rowCount() {
      return itemCount;
    },
    getRowCanExpand: () => true,
    autoResetExpanded: false
  });

  let initialized = $state(false);
  let showExpanded = $state(true);

  const handlePageChange = async (newPage: number) => {
    // Unmount the 'grandparent' block of the team-comp to skip the slide out transition
    showExpanded = false;
    await tick();

    // Reset the expanded IDs which would have otherwise caused a transition
    // table.toggleAllRowsExpanded doesn't seem to do anything
    expanded = {};

    const params = new SvelteURLSearchParams(page.url.searchParams);
    params.set('page', (newPage + 1).toString());

    // https://github.com/sveltejs/eslint-plugin-svelte/issues/1327
    // eslint-disable-next-line svelte/no-navigation-without-resolve
    await goto(`?${params.toString()}`, { noScroll: true });

    const el = document.querySelector('#time-attack-table-title');
    if (el) {
      el.scrollIntoView({ block: 'nearest' });
    }

    showExpanded = true;
  };

  onMount(() => {
    const params = new URLSearchParams(page.url.searchParams);
    const pageNumber = Number(params.get('page'));

    if (pageNumber) {
      table.setPageIndex(pageNumber - 1);
    }

    initialized = true;
  });
</script>

<div class="bg-card text-card-foreground rounded-xl border shadow-sm">
  <Table.Root id="time-attack-table" aria-labelledby="time-attack-table-title">
    <Table.Header id="time-attack-table-header" class="hidden md:[display:revert]">
      {#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
        <Table.Row>
          {#each headerGroup.headers as header (header.id)}
            <Table.Head>
              {#if !header.isPlaceholder}
                <FlexRender {header} />
              {/if}
            </Table.Head>
          {/each}
        </Table.Row>
      {/each}
    </Table.Header>
    <Table.Body>
      {#each table.getRowModel().rows as row (row.id)}
        <Table.Row class="flex flex-col md:[display:revert]">
          {#each row.getAllCells() as cell (cell.id)}
            {@const header = cell.column.columnDef.header}
            <Table.Cell class="px-4 py-3">
              <div class="text-muted-foreground md:hidden">
                {#if typeof header === 'string'}
                  {header}
                {/if}
              </div>
              <div>
                <FlexRender {cell} />
              </div>
            </Table.Cell>
          {/each}
        </Table.Row>
        <!--
          iOS Safari doesn't like it if you expand and close this section and starts rendering
          the rows side-by-side... avoiding the unmount of the extra <tr/> seems to fix this.
          The Blazor site used this kind of markup and that works fine. How mysterious!
           --->
        <tr aria-hidden={!row.getIsExpanded()}>
          {#if showExpanded}
            {#if row.getIsExpanded()}
              <td colspan="5">
                <div transition:slide={{ duration: 500 }} class="border-b p-4">
                  <TeamComposition
                    units={getTeam(coop, row.original.players)}
                    unitKeys={getTeamKeys(coop, row.original.players)}
                    key={row.original.rank}
                    {coop} />
                </div>
              </td>
            {/if}
          {/if}
        </tr>
      {/each}
    </Table.Body>
  </Table.Root>
  <div class="flex items-center justify-center space-x-4 py-2.5">
    <Button
      variant="outline"
      size="sm"
      onclick={() => table.previousPage()}
      disabled={!initialized || !table.getCanPreviousPage()}>
      Previous
    </Button>
    <Button
      variant="outline"
      size="sm"
      onclick={() => table.nextPage()}
      disabled={!initialized || !table.getCanNextPage()}>
      Next
    </Button>
  </div>
</div>
