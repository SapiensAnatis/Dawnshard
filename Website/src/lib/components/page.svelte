<script lang="ts">
  export let title: string;
</script>

<div class="px-6 pt-4 pb-2">
  <h1 class="text-4xl font-semibold tracking-tight">{title}</h1>
  <hr class="mt-2 mb-4" />
  <slot />
</div>
