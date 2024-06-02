import type { Icon } from 'lucide-svelte';
import type { ComponentType } from 'svelte';
import Newspaper from 'lucide-svelte/icons/newspaper';
import Home from 'lucide-svelte/icons/home';

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
      // @ts-expect-error https://github.com/lucide-icons/lucide/issues/2114
      { title: 'Home', href: '/', icon: Home },
      // @ts-expect-error https://github.com/lucide-icons/lucide/issues/2114
      { title: 'News', href: '/news/1', icon: Newspaper }
    ]
  }
];
