import { redirect } from '@sveltejs/kit';
import { Buffer } from 'buffer';

import { PUBLIC_BAAS_CLIENT_ID, PUBLIC_BAAS_URL } from '$env/static/public';
import Cookies from '$lib/auth/cookies.ts';

import type { PageServerLoad } from './$types';

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

export const load: PageServerLoad = async ({ cookies, locals, url }) => {
  const { logger } = locals;

  const redirectUri = new URL('oauth', url.origin);

  const originalPage = url.searchParams.get('originalPage') ?? '/';

  // For reasons not fully understood, going back to /oauth can sometimes run this code.
  // We don't want to overwrite the challenge string if we are about to send it to BaaS, because
  // that will mean the hashes won't match.
  let challengeStringValue = cookies.get(Cookies.ChallengeString);
  if (!challengeStringValue) {
    challengeStringValue = getChallengeString();
    cookies.set(Cookies.ChallengeString, challengeStringValue, { path: '/' });
  }

  const challengeStringHash = await getUrlSafeBase64Hash(challengeStringValue);

  logger.debug(
    { challengeStringValue, challengeStringHash },
    'Generated challenge string {challengeStringValue} with hash {challengeStringHash}'
  );

  const queryParams = new URLSearchParams({
    client_id: PUBLIC_BAAS_CLIENT_ID,
    redirect_uri: redirectUri.toString(),
    response_type: 'session_token_code',
    scope: 'user user.birthday openid',
    language: 'en-US',
    session_token_code_challenge: challengeStringHash,
    session_token_code_challenge_method: 'S256',
    state: JSON.stringify({ originalPage })
  });

  const baasUrl = new URL(`/custom/thirdparty/auth?${queryParams}`, PUBLIC_BAAS_URL);
  redirect(302, baasUrl);
};
