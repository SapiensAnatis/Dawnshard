<script lang="ts">
  import {
    type ColumnDef,
    type ExpandedState,
    getCoreRowModel,
    getPaginationRowModel,
    type PaginationState
  } from '@tanstack/table-core';
  import { onMount, tick } from 'svelte';
  import { slide } from 'svelte/transition';

  import { goto } from '$app/navigation';
  import { page } from '$app/state';
  import {
    createSvelteTable,
    FlexRender,
    renderComponent
  } from '$lib/shadcn/components/ui/data-table/index.js';
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

  const columns: ColumnDef<TimeAttackRanking>[] = [
    {
      accessorKey: 'rank',
      header: 'Rank'
    },
    {
      accessorKey: 'players',
      header: coop ? 'Players' : 'Player',
      cell: ({ getValue }) => {
        return getValue<TimeAttackPlayer[]>()
          .map((p) => p.name)
          .join(', ');
      }
    },
    {
      accessorKey: 'time',
      header: 'Clear Time',
      cell: ({ getValue }) => {
        const date = new Date(getValue<number>() * 1000);
        return date.toISOString().slice(14, -3);
      }
    },
    {
      id: 'team',
      accessorKey: 'players',
      header: 'Team',
      cell: ({ getValue }) => {
        const players = getValue<TimeAttackPlayer[]>();
        return renderComponent(TeamCell, { team: getTeam(coop, players), expanded: false });
      }
    },
    {
      id: 'expand',
      cell: ({ row }) => {
        return renderComponent(ExpandRow, {
          initialized,
          isExpanded: row.getIsExpanded(),
          toggleExpanded: row.toggleExpanded
        });
      }
    }
  ];

  const table = createSvelteTable({
    get data() {
      return data;
    },
    columns,
    state: {
      get pagination() {
        return pagination;
      },
      get expanded() {
        return expanded;
      }
    },
    onPaginationChange: (updaterOrValue) => {
      if (typeof updaterOrValue === 'function') {
        pagination = updaterOrValue(pagination);
      } else {
        pagination = updaterOrValue;
      }

      handlePageChange(pagination.pageIndex);
    },
    onExpandedChange: (updaterOrValue) => {
      if (typeof updaterOrValue === 'function') {
        expanded = updaterOrValue(expanded);
      } else {
        expanded = expanded;
      }
    },
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    manualPagination: true,
    rowCount: itemCount,
    enableExpanding: true
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

    const params = new URLSearchParams(page.url.searchParams);
    params.set('page', (newPage + 1).toString());

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

<div class="rounded-md border">
  <Table.Root id="time-attack-table" aria-labelledby="time-attack-table-title">
    <Table.Header id="time-attack-table-header" class="hidden md:[display:revert]">
      {#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
        <Table.Row>
          {#each headerGroup.headers as header (header.id)}
            <Table.Head>
              {#if !header.isPlaceholder}
                <FlexRender
                  content={header.column.columnDef.header}
                  context={header.getContext()} />
              {/if}
            </Table.Head>
          {/each}
        </Table.Row>
      {/each}
    </Table.Header>
    <Table.Body>
      {#each table.getRowModel().rows as row (row.id)}
        <Table.Row class="flex flex-col md:[display:revert]">
          {#each row.getVisibleCells() as cell (cell.id)}
            {@const header = cell.column.columnDef.header}
            <Table.Cell class="px-4 py-3">
              <div class="text-muted-foreground md:hidden">
                {#if typeof header === 'string'}
                  {header}
                {/if}
              </div>
              <div>
                <FlexRender content={cell.column.columnDef.cell} context={cell.getContext()} />
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
  <div class="flex items-center justify-center space-x-4 border-t py-2.5">
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
