import { type Icon } from 'lucide-svelte';
import ChartBarIncreasing from 'lucide-svelte/icons/chart-bar-increasing';
import House from 'lucide-svelte/icons/house';
import Newspaper from 'lucide-svelte/icons/newspaper';
import Pencil from 'lucide-svelte/icons/pencil';
import User from 'lucide-svelte/icons/user';
import VenetianMask from 'lucide-svelte/icons/venetian-mask';
import type { ComponentType } from 'svelte';

export type RouteGroup = {
  title: string;
  routes: Route[];
  requireAuth?: boolean;
  requireAdmin?: boolean;
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
      { title: 'Home', href: '/', icon: House },
      { title: 'News', href: '/news', icon: Newspaper }
    ]
  },

  {
    title: 'Events',
    routes: [
      {
        title: 'Time Attack Rankings',
        href: '/events/time-attack/rankings/227010104',
        icon: ChartBarIncreasing
      }
    ]
  },
  {
    title: 'Account',
    requireAuth: true,
    routes: [
      { title: 'Profile', href: '/account/profile', icon: User },
      {
        title: 'Save Editor',
        href: '/account/save-editor',
        icon: Pencil
      }
    ]
  },
  {
    title: 'Administration',
    requireAdmin: true,
    routes: [{ title: 'User Impersonation', href: '/admin/impersonation', icon: VenetianMask }]
  }
];
