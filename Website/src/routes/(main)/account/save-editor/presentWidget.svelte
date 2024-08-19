<script lang="ts">
  import Gift from 'lucide-svelte/icons/gift';
  import { createForm } from 'svelte-form-helper';

  import { Select } from '$lib/components/select';
  import { Button } from '$shadcn/components/ui/button/index.js';
  import * as Card from '$shadcn/components/ui/card';
  import { Input } from '$shadcn/components/ui/input';
  import { Label } from '$shadcn/components/ui/label';

  import { type EntityType, type PresentWidgetData } from './presentFormSchema.ts';

  export let widgetData: PresentWidgetData;

  $: availableItems = (typeValue && widgetData.availableItems[typeValue]) || [];
  $: disableQuantity =
    (typeValue && !widgetData.types.find((t) => t.value === typeValue)?.hasQuantity) || false;

  function onSubmit(evt) {
    console.log(typeValue, itemValue, quantityValue);
  }

  const validateQuantity = async (value: string) => {
    console.log(value, disableQuantity);
    if (disableQuantity) {
      return null;
    }

    return value ? null : 'value is required';
  };

  let typeValue: EntityType | undefined;
  let itemValue: number | undefined;
  let quantityValue: number = 1;

  const form = createForm();
  const type = form.field();
  const item = form.field();
  const quantity = form.field({ validator: validateQuantity, onDirty: true });

  $: {
    console.log($quantity);
  }
</script>

<Card.Root>
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Gift aria-hidden={true} size={25} />
        <h2 class="m-0 text-xl font-bold">Gift box</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <form use:form on:submit|preventDefault={onSubmit}>
      <p class="mb-5">Use this widget to add presents to your gift box.</p>
      <div class="flex flex-row gap-4">
        <div class="labelled-input">
          <Label for="type">Type</Label>
          <Select
            id="type"
            placeholder="Select an item type"
            items={widgetData.types}
            action={type}
            class="
              touched:invalid:text-red-700
              touched:invalid:border-red-700
            "
            required
            bind:value={typeValue} />
        </div>
        <div class="labelled-input">
          <Label for="item">Item</Label>
          <Select
            id="item"
            placeholder="Select an item"
            items={availableItems}
            action={item}
            required
            bind:value={itemValue} />
        </div>
        <div class="labelled-input">
          <Label>Quantity</Label>
          <Input
            id="quantity"
            placeholder="Enter a quantity"
            type="number"
            disabled={disableQuantity}
            action={quantity}
            bind:value={quantityValue} />
        </div>
      </div>
      <br />
      <Button type="submit" disabled={!$form.valid}>Add</Button>
    </form>
  </Card.Content>
</Card.Root>

<style>
  .labelled-input {
    width: 15rem;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }
</style>
