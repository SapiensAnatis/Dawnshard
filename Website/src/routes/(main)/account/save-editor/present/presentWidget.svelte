<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';
  import { createForm } from 'svelte-form-helper';

  import { Select } from '$lib/components/select';
  import { t } from '$lib/translations';
  import { formatTypeKey } from '$main/account/save-editor/present/util.ts';
  import { Button } from '$shadcn/components/ui/button/index.js';
  import * as Card from '$shadcn/components/ui/card';
  import { Input } from '$shadcn/components/ui/input';
  import { Label } from '$shadcn/components/ui/label';

  import { changesCount } from '../stores.ts';
  import { presents } from '../stores.ts';
  import {
    type EntityType,
    type PresentFormSubmission,
    type PresentWidgetData
  } from './presentTypes.ts';

  let { widgetData }: { widgetData: PresentWidgetData } = $props();

  let disableQuantity = $state(false);

  let typeValue: EntityType | '' = $state('');
  let itemValue: number | '' = $state('');
  let quantityValue: number = $state(1);
  let presentIdCounter: number = $state(0);

  const form = createForm();
  const type = form.field();
  const item = form.field();
  const quantity = form.field();

  const types = $derived(
    widgetData.types
      .map(({ type }) => ({
        value: type,
        label: $t(`entity.${formatTypeKey(type)}.label`)
      }))
      .sort((a, b) => a.label.localeCompare(b.label))
  );

  const availableItems = $derived.by(() => {
    if (!typeValue) {
      return [];
    }

    const itemList = widgetData.availableItems[typeValue];

    if (!itemList) {
      return [];
    }

    return itemList
      .map(({ id }) => ({
        value: id,
        label: $t(`entity.${formatTypeKey(typeValue)}.item.${id}`)
      }))
      .sort((a, b) => a.label.localeCompare(b.label));
  });

  const disableItem = $derived(availableItems.length > 0 ? undefined : 'true');

  const onSubmit = (evt: SubmitEvent) => {
    evt.preventDefault();

    if (typeValue === '' || itemValue === '') return;

    const submission: PresentFormSubmission = {
      id: ++presentIdCounter,
      type: typeValue,
      item: itemValue,
      quantity: quantityValue
    };

    presents.update((existing) => [...existing, submission]);
  };

  const onTypeChange = () => {
    if (!typeValue) return;

    disableQuantity = !widgetData.types.find((t) => t.type === typeValue)?.hasQuantity;

    if (disableQuantity) {
      quantityValue = 1;
    }

    itemValue = '';
  };
</script>

<Card.Root>
  <Card.Header>
    <Card.Title id="present-widget-title">
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Gift aria-hidden={true} size={25} />
        Gift box
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <p class="mb-5">Use this widget to add presents to your gift box.</p>
    <form use:form onsubmit={onSubmit} aria-labelledby="present-widget-title">
      <div class="flex flex-row flex-wrap gap-4">
        <div class="labelled-input">
          <Label for="type">Type</Label>
          <Select
            id="type"
            placeholder="Select an item type"
            items={types}
            field={type}
            on:change={onTypeChange}
            class="
              touched:invalid:border-red-700
              touched:invalid:text-red-700
            "
            required
            bind:value={typeValue} />
          {#if $type.show}
            <p class="helper">{$type.message}</p>
          {/if}
        </div>
        <div class="labelled-input">
          <Label for="item">Item</Label>
          <Select
            id="item"
            placeholder="Select an item"
            items={availableItems}
            disabled={disableItem}
            field={item}
            required
            class="touched:invalid:border-red-700 touched:invalid:text-red-700"
            bind:value={itemValue} />
          {#if $item.show}
            <p class="helper">{$item.message}</p>
          {/if}
        </div>
        <div class="labelled-input">
          <Label for="quantity">Quantity</Label>
          <Input
            id="quantity"
            placeholder="Enter a quantity"
            type="number"
            disabled={disableQuantity}
            field={quantity}
            min={1}
            max={999999}
            required
            class="
              touched:invalid:border-red-700
              touched:invalid:text-red-700
            "
            bind:value={quantityValue} />
          {#if $quantity.show}
            <p class="helper">{$quantity.message}</p>
          {/if}
        </div>
      </div>
      <br />
      <Button type="submit" disabled={!$form.valid || !$form.touched || $changesCount >= 100}>
        Add
      </Button>
    </form>
  </Card.Content>
</Card.Root>

<style>
  .labelled-input {
    width: 12.5rem;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .helper {
    font-size: 0.75rem;
    color: hsl(var(--muted-foreground));
  }
</style>
