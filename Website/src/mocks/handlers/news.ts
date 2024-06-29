import { HttpResponse, type HttpResponseResolver } from 'msw';

const newsItems = [
  {
    id: 6,
    headline: 'Game updated!',
    description:
      'We have done a very large update! Almost all of the server functionality is broken, but we have been able to add micro-transactions. Please address any comments or concerns to your nearest brick wall.',
    date: '2024-06-28T22:43:49Z',
    headerImageSrc:
      'https://images.unsplash.com/photo-1520758594221-872948699332?q=80&w=1000&h=1000&fit=crop',
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
  }
];

const handleNews: HttpResponseResolver = ({ params }) => {
  return HttpResponse.json({
    pagination: {
      totalCount: 1
    },
    data: newsItems
  });
};

export default handleNews;
