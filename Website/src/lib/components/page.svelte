<script lang="ts">
  export let title: string;
</script>

<div class="px-6 pb-2 pt-4">
  <h1 class="text-4xl font-semibold tracking-tight">{title}</h1>
  <hr class="mb-4 mt-2" />
  <slot />
</div>
