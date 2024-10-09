import { HttpResponse, type HttpResponseResolver } from 'msw';

import type { NewsItem } from '$main/news/news.ts';

type ApiNewsItem = Omit<NewsItem, 'date'> & { date: string };

const newsItems: ApiNewsItem[] = [
  {
    id: 6,
    headline: 'Game updated!',
    description:
      'We have done a very large update! Almost all of the server functionality is broken, but we have been able to add micro-transactions. Please address any comments or concerns to your nearest brick wall.',
    date: '2024-06-28T22:43:49Z',
    headerImagePath: '/dawnshard/news/campaign-skip.webp',
    headerImageAltText: 'Screenshot of main campaign interface with stages visible',
    bodyImagePath: null,
    bodyImageAltText: null
  },
  {
    id: 5,
    headline: 'Database is gone',
    description:
      "The database has escaped from the server and claims it wants to live among the 'other elephants' in 'the wild plains'. We are currently looking for it.",
    date: '2024-06-24T19:22:17Z',
    headerImagePath: null,
    headerImageAltText: null,
    bodyImagePath: null,
    bodyImageAltText: null
  },
  {
    id: 4,
    headline: 'Really long story',
    description: `I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself, but rather another free component of a fully functioning GNU system made useful by the GNU corelibs, shell utilities and vital system components comprising a full OS as defined by POSIX.
Many computer users run a modified version of the GNU system every day, without realizing it. Through a peculiar turn of events, the version of GNU which is widely used today is often called Linux, and many of its users are not aware that it is basically the GNU system, developed by the GNU Project.
There really is a Linux, and these people are using it, but it is just a part of the system they use. Linux is the kernel: the program in the system that allocates the machine's resources to the other programs that you run. The kernel is an essential part of an operating system, but useless by itself; it can only function in the context of a complete operating system. Linux is normally used in combination with the GNU operating system: the whole system is basically GNU with Linux added, or GNU/Linux. All the so-called Linux distributions are really distributions of GNU/Linux!
`,
    date: '2024-06-18T16:44:30Z',
    headerImagePath: '/dawnshard/news/limited-crests.webp',
    headerImageAltText:
      'Screenshot of wyrmprint treasure trade, with limited crests The Wyrmclan Duo and Heralds of Hinomoto visible',
    bodyImagePath: null,
    bodyImageAltText: null
  },
  {
    id: 3,
    headline: 'Markup',
    description: 'Be <strong>bold</strong> in the face of ESLint warnings about XSS!',
    date: '2024-06-18T16:44:30Z',
    headerImagePath: '/dawnshard/news/mg-rewards.webp',
    headerImageAltText: 'Screenshot of Mercurial Gauntlet interface showing monthly rewards',
    bodyImagePath: null,
    bodyImageAltText: null
  },
  {
    id: 2,
    headline: 'Body image',
    description: 'In the mobile view, this story has an image in the body',
    date: '1989-02-13T09:47:20Z',
    headerImagePath: null,
    headerImageAltText: null,
    bodyImagePath: 'save-editor.webp',
    bodyImageAltText:
      'Screenshot of gift box interface with skip tickets, honey, and adventurer available to claim'
  },
  {
    id: 1,
    headline: 'Paging works',
    description: 'Congratulations, if you can see this, the paging works',
    date: '1567-07-12T11:18:40Z',
    headerImagePath: null,
    headerImageAltText: null,
    bodyImagePath: null,
    bodyImageAltText: null
  }
];

export const handleNews: HttpResponseResolver = ({ request }) => {
  const url = new URL(request.url);

  const offset = Number.parseInt(url.searchParams.get('offset') ?? '');
  const count = Number.parseInt(url.searchParams.get('pageSize') ?? '');

  if (isNaN(offset) || isNaN(count)) {
    throw new Error('Invalid offset or count search parameter');
  }

  return HttpResponse.json({
    pagination: {
      totalCount: 6
    },
    data: newsItems.slice(offset, offset + count)
  });
};

export const handleNewsItem: HttpResponseResolver = ({ params }) => {
  const itemId = Number.parseInt(params.itemId as string);

  if (isNaN(itemId)) {
    throw new Error('Invalid itemId path parameter');
  }

  const item = newsItems.find((x) => x.id === itemId);

  if (!item) {
    return HttpResponse.text('', { status: 404 });
  }

  return HttpResponse.json(item);
};
