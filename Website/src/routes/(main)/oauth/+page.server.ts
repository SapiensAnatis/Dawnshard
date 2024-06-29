import { type Cookies, redirect } from '@sveltejs/kit';
import { z } from 'zod';

import {
  PUBLIC_BAAS_CLIENT_ID,
  PUBLIC_BAAS_URL,
  PUBLIC_DAWNSHARD_API_URL,
  PUBLIC_ENABLE_MSW
} from '$env/static/public';
import CookieNames from '$lib/auth/cookies.ts';
import getJwtMetadata from '$lib/auth/jwt.ts';

import type { PageServerLoad } from './$types';

const sessionTokenUrl = new URL('/connect/1.0.0/api/session_token', PUBLIC_BAAS_URL);
const sdkTokenUrl = new URL('/1.0.0/gateway/sdk/token', PUBLIC_BAAS_URL);

const sessionTokenResponseSchema = z.object({
  session_token: z.string()
});

const sdkTokenResponseSchema = z.object({
  idToken: z.string()
});

export const load: PageServerLoad = async ({ cookies, url, fetch }) => {
  const idToken = await getBaasToken(cookies, url, fetch);

  const jwtMetadata = getJwtMetadata(idToken);
  if (!jwtMetadata.valid) {
    throw Error('Invalid JWT returned');
  }

  const maxAge = (jwtMetadata.expiryTimestampMs - Date.now()) / 1000;

  if (!(await checkUserExists(idToken, fetch))) {
    redirect(302, '/login/unauthorized/404');
  }

  cookies.set(CookieNames.IdToken, idToken, {
    path: '/',
    maxAge,
    httpOnly: false,
    ...(!PUBLIC_ENABLE_MSW && {
      sameSite: 'lax',
      httpOnly: true,
      secure: true
    })
  });

  cookies.delete('challengeString', {
    path: '/'
  });

  const destination = getOriginalPage(url) ?? '/';

  if (destination.includes('unauthorized')) {
    redirect(302, '/');
  }

  redirect(302, destination);
};

const getBaasToken = async (
  cookies: Cookies,
  url: URL,
  fetch: (url: URL, req: RequestInit) => Promise<Response>
) => {
  const challengeString = cookies.get(CookieNames.ChallengeString);

  if (!challengeString) {
    throw new Error('Failed to get challenge string');
  }

  const sessionTokenCode = url.searchParams.get('session_token_code');

  if (!sessionTokenCode) {
    throw new Error('Failed to get session token code');
  }

  const sessionTokenCodeParams = new URLSearchParams({
    client_id: PUBLIC_BAAS_CLIENT_ID,
    session_token_code: sessionTokenCode,
    session_token_code_verifier: challengeString
  });

  const sessionTokenResponse = await fetch(sessionTokenUrl, {
    method: 'POST',
    body: sessionTokenCodeParams
  });

  if (!sessionTokenResponse.ok) {
    throw new Error('Session token request failed');
  }

  const { session_token: sessionToken } = sessionTokenResponseSchema.parse(
    await sessionTokenResponse.json()
  );

  const sdkTokenRequest = {
    client_id: PUBLIC_BAAS_CLIENT_ID,
    session_token: sessionToken
  };

  const sdkTokenResponse = await fetch(sdkTokenUrl, {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(sdkTokenRequest)
  });

  if (!sdkTokenResponse.ok) {
    throw new Error('SDK token request failed');
  }

  const { idToken } = sdkTokenResponseSchema.parse(await sdkTokenResponse.json());
  return idToken;
};

const checkUserExists = async (
  idToken: string,
  fetch: (url: URL, req: RequestInit) => Promise<Response>
) => {
  const userMeResponse = await fetch(new URL('user/me', PUBLIC_DAWNSHARD_API_URL), {
    method: 'HEAD',
    headers: {
      Authorization: `Bearer ${idToken}`
    }
  });

  if (userMeResponse.ok) {
    return true;
  } else if (userMeResponse.status === 404) {
    return false;
  } else {
    throw new Error(`Unexpected /user/me response in OAuth callback: ${userMeResponse.status}`);
  }
};

const getOriginalPage = (url: URL) => {
  const stateJson = url.searchParams.get('state');
  if (!stateJson) {
    return null;
  }

  let stateObject;
  try {
    stateObject = JSON.parse(stateJson);
  } catch {
    return null;
  }

  if (!stateObject.originalPage) {
    return null;
  }

  return stateObject.originalPage;
};
