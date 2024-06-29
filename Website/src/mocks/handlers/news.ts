import { HttpResponse, type HttpResponseResolver } from 'msw';

const newsItems = [
  {
    id: 6,
    headline: 'Game updated!',
    description:
      'We have done a very large update! Almost all of the server functionality is broken, but we have been able to add micro-transactions. Please address any comments or concerns to your nearest brick wall.',
    date: '2024-06-28T22:43:49Z',
    headerImageSrc:
      'https://images.unsplash.com/photo-1520758594221-872948699332?q=30&w=1000&h=1000&fit=crop',
    bodyImageSrc: null
  },
  {
    id: 5,
    headline: 'Database is gone',
    description:
      "The database has escaped from the server and claims it wants to live among the 'other elephants' in 'the wild plains'. We are currently looking for it.",
    date: '2024-06-24T19:22:17Z',
    headerImageSrc: null,
    bodyImageSrc: null
  },
  {
    id: 4,
    headline: 'Really long story',
    description: `I'd just like to interject for a moment. What you're referring to as Linux, is in fact, GNU/Linux, or as I've recently taken to calling it, GNU plus Linux. Linux is not an operating system unto itself, but rather another free component of a fully functioning GNU system made useful by the GNU corelibs, shell utilities and vital system components comprising a full OS as defined by POSIX.
Many computer users run a modified version of the GNU system every day, without realizing it. Through a peculiar turn of events, the version of GNU which is widely used today is often called Linux, and many of its users are not aware that it is basically the GNU system, developed by the GNU Project.
There really is a Linux, and these people are using it, but it is just a part of the system they use. Linux is the kernel: the program in the system that allocates the machine's resources to the other programs that you run. The kernel is an essential part of an operating system, but useless by itself; it can only function in the context of a complete operating system. Linux is normally used in combination with the GNU operating system: the whole system is basically GNU with Linux added, or GNU/Linux. All the so-called Linux distributions are really distributions of GNU/Linux!
`,
    date: '2024-06-18T16:44:30Z',
    headerImageSrc:
      'https://upload.wikimedia.org/wikipedia/commons/thumb/3/35/Tux.svg/800px-Tux.svg.png',
    bodyImageSrc: null
  },
  {
    id: 3,
    headline: 'Markup',
    description: 'Be <strong>bold</strong> in the face of ESLint warnings about XSS!',
    date: '2024-06-18T16:44:30Z',
    headerImageSrc:
      'https://images.unsplash.com/photo-1624969862644-791f3dc98927?q=30&w=1000&auto=format&fit=crop',
    bodyImageSrc: null
  },
  {
    id: 2,
    headline: 'Body image',
    description: 'In the mobile view, this story has an image in the body',
    headerImageSrc:
      'https://oldschool.runescape.wiki/images/thumb/Body_rune_detail.png/1024px-Body_rune_detail.png',
    bodyImageSrc:
      'https://images.unsplash.com/photo-1501869150797-9bbb64f782fd?q=80&w=1974&auto=format&fit=crop',
    date: '1989-02-13T09:47:20Z'
  },
  {
    id: 1,
    headline: 'Paging works',
    description: 'Congratulations, if you can see this, the paging works',
    date: '1567-07-12T11:18:40Z',
    headerImageSrc: null,
    bodyImageSrc: null
  }
];

const handleNews: HttpResponseResolver = ({ request }) => {
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

export default handleNews;
