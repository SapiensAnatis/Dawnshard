<script lang="ts">
  import * as Form from '$shadcn/components/ui/form';
  import { Input } from '$shadcn/components/ui/input';
  import { presentFormSchema, type PresentFormSchema } from './presentFormSchema.ts';
  import { type SuperValidated, type Infer, superForm } from 'sveltekit-superforms';
  import { zodClient } from 'sveltekit-superforms/adapters';

  import Gift from 'lucide-svelte/icons/gift';

  import { Select, type SelectItem } from '$lib/components/select';
  import { Button } from '$shadcn/components/ui/button/index.js';
  import * as Card from '$shadcn/components/ui/card';

  import { type EntityType, type PresentWidgetData } from './presentFormSchema.ts';
  import { Input } from '$shadcn/components/ui/input';
  import { Label } from '$shadcn/components/ui/label';

  export let widgetData: PresentWidgetData;
  export let data: SuperValidated<Infer<PresentFormSchema>>;

  const form = superForm(data, {
    validators: zodClient(presentFormSchema)
  });

  const { form: formData, enhance } = form;

  $: availableItems = $formData.type ? (widgetData.availableItems[$formData.type] ?? []) : [];
  $: selectedTypeObject = $formData.type
    ? widgetData.types.find((type) => type.value === $formData.type)
    : null;
  $: quantityInputDisabled = !selectedTypeObject?.hasQuantity;

  $: {
    if (quantityInputDisabled && $formData.quantity !== 1) {
      $formData.quantity = 1;
    }
  }

  // todo : use https://www.npmjs.com/package/svelte-form-helper
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
    <form use:enhance>
      <p class="mb-5">Use this widget to add presents to your gift box.</p>
      <div class="flex flex-row gap-4">
        <Form.Field {form} name="type">
          <Form.Control></Form.Control>
          <Form.Label>Type</Form.Label>
          <Select items={widgetData.types} {...attrs} bind:value={$formData.type} />
        </Form.Field>
        <Form.Control let:attrs>
          <Form.Label>Item</Form.Label>
          <Select items={availableItems} {...attrs} bind:value={$formData.item} />
        </Form.Control>
        <Form.Control let:attrs>
          <Form.Label>Quantity</Form.Label>
          <Select items={availableItems} {...attrs} bind:value={$formData.quantity} />
        </Form.Control>
        <Form.FieldErrors />
      </div>
      <br />
      <Button>Add</Button>
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
