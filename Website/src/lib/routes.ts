import type { Icon } from 'lucide-svelte';
import type { ComponentType } from 'svelte';
import * as Icons from './icons';

export type RouteGroup = {
	title: string;
	routes: Route[];
};

export type Route = {
	title: string;
	href: string;
	icon: ComponentType<Icon>;
};

export const routeGroups: RouteGroup[] = [
	{
		title: 'Information',
		routes: [
			{ title: 'Home', href: '/', icon: Icons.Home },
			{ title: 'News', href: '/news', icon: Icons.Newspaper }
		]
	}
];
