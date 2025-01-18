import type { RequestHandler } from './$types';

export const POST: RequestHandler = async ({ locals, request }) => {
  let violation;

  try {
    violation = await request.json();
  } catch (error) {
    locals.logger.debug({ error }, 'Invalid CSP violation report: {error}');
    return new Response(null, { status: 400 });
  }

  locals.logger.error({ violation }, 'CSP violation reported: {violation}');

  return new Response(null, { status: 200 });
};
