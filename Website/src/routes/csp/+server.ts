import type { RequestHandler } from './$types';

export const POST: RequestHandler = async ({ locals, request }) => {
  let violation;

  try {
    violation = await request.json();
  } catch (error) {
    locals.logger.debug({ error }, 'Invalid CSP violation report: {error}');
    return new Response(null, { status: 400 });
  }

  const userAgent = request.headers.get('User-Agent');

  locals.logger.error(
    { violation, userAgent },
    'CSP violation reported: {violation}. User-Agent: {userAgent}'
  );

  return new Response(null, { status: 200 });
};
