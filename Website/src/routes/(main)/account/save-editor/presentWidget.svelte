<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';
  import { createForm } from 'svelte-form-helper';

  import { Select } from '$lib/components/select';
  import { Button } from '$shadcn/components/ui/button/index.js';
  import * as Card from '$shadcn/components/ui/card';
  import { Input } from '$shadcn/components/ui/input';
  import { Label } from '$shadcn/components/ui/label';

  import {
    type EntityType,
    type PresentFormSubmission,
    type PresentWidgetData
  } from './presentTypes.ts';
  import { presents } from './stores.ts';

  export let widgetData: PresentWidgetData;

  $: availableItems = (typeValue && widgetData.availableItems[typeValue]) || [];
  $: disableQuantity =
    (typeValue && !widgetData.types.find((t) => t.value === typeValue)?.hasQuantity) || false;

  const onSubmit = () => {
    const itemLabel = availableItems.find((i) => i.value === itemValue)?.label;

    if (!typeValue || !itemValue || !itemLabel) return;

    const submission: PresentFormSubmission = {
      type: typeValue,
      item: itemValue,
      itemLabel,
      quantity: quantityValue
    };

    presents.update((existing) => [...existing, submission]);
  };

  let typeValue: EntityType | '';
  let itemValue: number | '';
  let quantityValue: number = 1;

  const form = createForm();
  const type = form.field();
  const item = form.field();
  const quantity = form.field();
</script>

<Card.Root>
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Gift aria-hidden={true} size={25} />
        <h2 id="gift-box-title" class="m-0 text-xl font-bold">Gift box</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <p class="mb-5">Use this widget to add presents to your gift box.</p>
    <form use:form on:submit|preventDefault={onSubmit} aria-labelledby="gift-box-title">
      <div class="flex flex-row flex-wrap gap-4">
        <div class="labelled-input">
          <Label for="type">Type</Label>
          <Select
            id="type"
            placeholder="Select an item type"
            items={widgetData.types}
            action={type}
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
            action={item}
            required
            class="
              touched:invalid:border-red-700
              touched:invalid:text-red-700
            "
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
            action={quantity}
            min={1}
            max={2147483647}
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
      <Button type="submit" disabled={!$form.valid || !$form.touched}>Add</Button>
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
