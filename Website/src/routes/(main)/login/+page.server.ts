import { redirect } from '@sveltejs/kit';
import { Buffer } from 'buffer';
import type { PageServerLoad } from './$types';
import { PUBLIC_BAAS_URL, PUBLIC_BAAS_CLIENT_ID, PUBLIC_DAWNSHARD_URL } from '$env/static/public';

const redirectUri = new URL('oauth', PUBLIC_DAWNSHARD_URL);

const getChallengeString = () => {
  const buffer = new Uint8Array(8);
  crypto.getRandomValues(buffer);
  return Array.from(buffer, (dec) => dec.toString(16).padStart(2, '0')).join('');
};

const getUrlSafeBase64Hash = async (input: string) => {
  const buffer = new TextEncoder().encode(input);
  const hashBuffer = await crypto.subtle.digest('SHA-256', buffer);
  const base64 = Buffer.from(new Uint8Array(hashBuffer)).toString('base64');
  return base64.replace('+', '-').replace('/', '_').replace('=', '');
};

export const load: PageServerLoad = async ({ cookies, url }) => {
  const originalPage = url.searchParams.get('originalPage') ?? '/';

  const challengeStringValue = getChallengeString();
  cookies.set('challengeString', challengeStringValue, { path: '/' });

  const queryParams = new URLSearchParams({
    client_id: PUBLIC_BAAS_CLIENT_ID,
    redirect_uri: redirectUri.toString(),
    response_type: 'session_token_code',
    scope: 'user user.birthday openid',
    language: 'en-US',
    session_token_code_challenge: await getUrlSafeBase64Hash(challengeStringValue),
    session_token_code_challenge_method: 'S256',
    state: JSON.stringify({ originalPage })
  });

  const baasUrl = new URL(`/custom/thirdparty/auth?${queryParams}`, PUBLIC_BAAS_URL);
  redirect(302, baasUrl);
};
