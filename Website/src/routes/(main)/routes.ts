import type { Icon } from 'lucide-svelte';
import Home from 'lucide-svelte/icons/home';
import Newspaper from 'lucide-svelte/icons/newspaper';
import User from 'lucide-svelte/icons/user';
import type { ComponentType } from 'svelte';

export type RouteGroup = {
  title: string;
  routes: Route[];
  requireAuth?: boolean;
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
  },
  {
    title: 'Account',
    requireAuth: true,
    routes: [
      // @ts-expect-error https://github.com/lucide-icons/lucide/issues/2114
      { title: 'Profile', href: '/account/profile', icon: User }
    ]
  }
];
