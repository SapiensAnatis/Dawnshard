<script lang="ts">
  import { readable } from 'svelte/store';
  import { slide } from 'svelte/transition';
  import { createRender, createTable, Render, Subscribe } from 'svelte-headless-table';
  import { addExpandedRows } from 'svelte-headless-table/plugins';

  import {
    getTeam,
    getTeamKeys
  } from '$main/events/time-attack/rankings/[questId=integer]/util.ts';
  import type { TimeAttackRanking } from '$main/events/time-attack/rankings/timeAttackTypes.ts';
  import * as Table from '$shadcn/components/ui/table';

  import TeamCell from './teamCell.svelte';
  import TeamComposition from './teamComposition/teamComposition.svelte';

  export let data: TimeAttackRanking[];
  export let coop: boolean = false;

  const table = createTable(readable(data), {
    expand: addExpandedRows()
  });

  const columns = table.createColumns([
    table.column({
      accessor: ({ rank }) => rank,
      header: 'Rank'
    }),
    table.column({
      accessor: ({ players }) => players.map((p) => p.name).join(', '),
      header: coop ? 'Players' : 'Player'
    }),
    table.column({
      accessor: ({ time }) => time,
      cell: ({ value: time }) => {
        const minutes = `${Math.floor(time / 60)}`.padStart(2, '0');
        const seconds = `${time % 60}`.padStart(2, '0').slice(0, 4);
        return `${minutes}:${seconds}`;
      },
      header: 'Clear Time'
    }),
    table.column({
      accessor: ({ players }) => {
        return getTeam(coop, players);
      },
      header: 'Team',
      cell: ({ row, value }, { pluginStates }) => {
        const { isExpanded } = pluginStates.expand.getRowState(row);
        return createRender(TeamCell, { team: value, isExpanded });
      }
    })
  ]);

  const { headerRows, pageRows, tableAttrs, tableBodyAttrs, tableHeadAttrs, pluginStates } =
    table.createViewModel(columns);

  let expandedIds = pluginStates.expand.expandedIds;
</script>

<div class="rounded-md border">
  <Table.Root {...$tableAttrs} id="time-attack-table">
    <Table.Header {...$tableHeadAttrs} id="header" class="hidden md:[display:revert]">
      {#each $headerRows as headerRow}
        <Subscribe rowAttrs={headerRow.attrs()}>
          <Table.Row>
            {#each headerRow.cells as cell (cell.id)}
              <Subscribe attrs={cell.attrs()} let:attrs props={cell.props()}>
                <Table.Head {...attrs}>
                  <Render of={cell.render()} />
                </Table.Head>
              </Subscribe>
            {/each}
          </Table.Row>
        </Subscribe>
      {/each}
    </Table.Header>
    <Table.Body {...$tableBodyAttrs}>
      {#each $pageRows as row (row.id)}
        <Subscribe rowAttrs={row.attrs()} let:rowAttrs>
          <Table.Row {...rowAttrs} class="flex flex-col md:[display:revert]">
            {#each $headerRows[0].cells.map( (header, i) => ({ header, cell: row.cells[i] }) ) as { cell, header }}
              <Subscribe attrs={cell.attrs()} let:attrs>
                <Table.Cell {...attrs} class="px-4 py-3">
                  <div class="text-muted-foreground md:hidden">
                    <Render of={header.render()} />
                  </div>
                  <div>
                    <Render of={cell.render()} />
                  </div>
                </Table.Cell>
              </Subscribe>
            {/each}
          </Table.Row>
          {#if $expandedIds[row.id] && row.isData()}
            <tr class="border-b">
              <td colspan="4">
                <div transition:slide={{ duration: 500 }} class="p-4">
                  <TeamComposition
                    units={getTeam(coop, row.original.players)}
                    unitKeys={getTeamKeys(coop, row.original.players)} />
                </div>
              </td>
            </tr>
          {/if}
        </Subscribe>
      {/each}
    </Table.Body>
  </Table.Root>
</div>
