<script lang="ts">
  import { cn } from '$shadcn/utils.ts';

  export let size: number = 24;
  let className: string | undefined = undefined;

  export { className as class };
</script>

<svg
  xmlns="http://www.w3.org/2000/svg"
  width={size}
  height={size}
  viewBox="0 0 24 24"
  fill="none"
  stroke="currentColor"
  stroke-width="2"
  stroke-linecap="round"
  stroke-linejoin="round"
  class={cn('animate-spin', className)}>
  <path d="M21 12a9 9 0 1 1-6.219-8.56" />
</svg>
