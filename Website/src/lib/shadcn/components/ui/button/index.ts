import Root, {
	// @ts-expect-error unable to import ButtonProps https://github.com/huntabyte/shadcn-svelte/issues/1468
	type ButtonProps,
	// @ts-expect-error unable to export ButtonSize
	type ButtonSize,
	// @ts-expect-error unable to export ButtonVariant
	type ButtonVariant,
	// @ts-expect-error unable to import buttonVariants
	buttonVariants,
} from "./button.svelte";

export {
	Root,
	type ButtonProps as Props,
	//
	Root as Button,
	buttonVariants,
	type ButtonProps,
	type ButtonSize,
	type ButtonVariant,
};
