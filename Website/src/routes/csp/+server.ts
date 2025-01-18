import type { RequestHandler } from './$types';

export const POST: RequestHandler = async ({ locals, request }) => {
  const violation = await request.json();

  locals.logger.error({ violation }, 'CSP violation reported: {violation}');

  return new Response(null, { status: 200 });
};
