<script lang="ts" generics="TData extends RowData & { itemId: number }">
 import {
  type ColumnDef,
  type RowData,
  createTable,
  FlexRender,
 } from "@tanstack/svelte-table";
 import * as Table from "$lib/components/ui/table/index.js";
 import { features, type DataTableFeatures } from "./data-table-features.js";
import { Button } from "$lib/components/ui/button/index.js";
 import { tick } from "svelte";
 type DataTableProps<TData extends RowData> = {
  columns: ColumnDef<DataTableFeatures, TData>[];
  data: TData[];
 };
 
 let { data, columns }: DataTableProps<TData> = $props();
 
 const table = createTable({
  features,
  enableSortingRemoval: false,
  get data() {
   return data;
  },
  columns,
 });

  // Rune-reactive read — updates whenever the user changes pages.
  const pagination = $derived(table.atoms.pagination.get());
  $effect(() => {
   // Make the effect react to changes in the rendered table.
    // eslint-disable-next-line @typescript-eslint/no-unused-expressions
   table.getRowModel().rows;

   tick().then(() => {
    if (typeof window !== "undefined" && window.$WowheadPower) {
     window.$WowheadPower.refreshLinks();
    }
   });
  });
</script>
 
<div class="rounded-md border bg-taupe-800">
 <Table.Root>
  <Table.Header>
   {#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
    <Table.Row>
     {#each headerGroup.headers as header (header.id)}
      <Table.Head colspan={header.colSpan} class="text-white text-lg">
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
		<Table.Row data-state={row.getIsSelected() && "selected"}>
			{#each row.getVisibleCells() as cell (cell.id)}
				<Table.Cell>
					{#if cell.column.id === 'itemName'}
						<a
							href={`https://www.wowhead.com/item=${row.original.itemId}`}
              data-wowhead={`item=${row.original.itemId}`}
							target="_blank"
							rel="noopener noreferrer"
                            class="text-lg"
						>
							<FlexRender {cell} />
						</a>
					{:else}
                    <span class="text-white text-lg">
						<FlexRender {cell} />
                    </span>
					{/if}
				</Table.Cell>
			{/each}
		</Table.Row>
	{:else}
		<Table.Row>
			<Table.Cell colspan={columns.length} class="h-24 text-center">
				No results.
			</Table.Cell>
		</Table.Row>
	{/each}
</Table.Body>
 </Table.Root>
</div>
<div class="flex items-center justify-between py-4">
  <div class="text-sm">
    Page {pagination.pageIndex + 1} of {table.getPageCount()}
  </div>

  <div class="flex items-center space-x-2">
    <Button
      variant="outline"
      size="sm"
      onclick={() => table.previousPage()}
      disabled={!table.getCanPreviousPage()}
    >
      Previous
    </Button>
    <Button
      variant="outline"
      size="sm"
      onclick={() => table.nextPage()}
      disabled={!table.getCanNextPage()}
    >
      Next
    </Button>
  </div>
</div>