import { error, redirect } from '@sveltejs/kit';
import type Logger from 'bunyan';
import { z } from 'zod';

import { PUBLIC_BAAS_CLIENT_ID, PUBLIC_BAAS_URL, PUBLIC_ENABLE_MSW } from '$env/static/public';
import Cookies from '$lib/auth/cookies.ts';
import getJwtMetadata from '$lib/auth/jwt.ts';
import { userSchema } from '$main/account/user.ts';

import type { PageServerLoad } from './$types';

const sessionTokenUrl = new URL('/connect/1.0.0/api/session_token', PUBLIC_BAAS_URL);
const sdkTokenUrl = new URL('/1.0.0/gateway/sdk/token', PUBLIC_BAAS_URL);

const sessionTokenResponseSchema = z.object({
  session_token: z.string()
});

const sdkTokenResponseSchema = z.object({
  idToken: z.string()
});

const baseCookieSettings = Object.freeze({
  path: '/',
  httpOnly: false,
  ...(!PUBLIC_ENABLE_MSW && {
    sameSite: 'lax',
    httpOnly: true,
    secure: true
  })
});

export const load: PageServerLoad = async ({ cookies, locals, url, fetch }) => {
  const { logger } = locals;

  const sessionTokenCode = url.searchParams.get('session_token_code');

  if (!sessionTokenCode) {
    error(400, 'Missing session_token_code query parameter');
  }

  logger.debug(
    { stcMetadata: getJwtMetadata(sessionTokenCode) },
    'Retrieved session token code with metadata {stcMetadata}'
  );

  const challengeString = cookies.get(Cookies.ChallengeString);
  logger.debug({ challengeString }, 'Retrieved challenge string: {challengeString}');

  if (!challengeString) {
    error(
      400,
      'Missing challengeString cookie. ' +
        'Ensure that your browser is able to accept cookies from this website.'
    );
  }

  const idToken = await getBaasToken(sessionTokenCode, challengeString, fetch, logger);

  const jwtMetadata = getJwtMetadata(idToken);
  if (!jwtMetadata.valid) {
    throw Error('Invalid JWT returned');
  }

  const maxAge = (jwtMetadata.expiryTimestampMs - Date.now()) / 1000;

  const userInfo = await getUserInfo(idToken, url, fetch);

  if (!userInfo) {
    redirect(302, '/unauthorized/404');
  }

  const cookieSettings = { ...baseCookieSettings, maxAge };

  cookies.set(Cookies.IdToken, idToken, cookieSettings);
  cookies.set(Cookies.Claims, JSON.stringify(userInfo.claims), cookieSettings);

  cookies.delete('challengeString', {
    path: '/'
  });

  const originalPage = getOriginalPage(url) ?? '/';

  if (originalPage.includes('unauthorized')) {
    redirect(302, '/');
  }

  redirect(302, originalPage);
};

const getBaasToken = async (
  sessionTokenCode: string,
  challengeString: string,
  fetch: (url: URL, req: RequestInit) => Promise<Response>,
  logger: Logger
) => {
  logger.debug({ currentTimestamp: Date.now() }, 'Current timestamp: {currentTimestamp}');

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
    logger.error(
      { status: sessionTokenResponse.status },
      'Session token request failed with status {status}'
    );

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
    logger.error(
      { status: sdkTokenResponse.status },
      'SDK token request failed with status {status}'
    );

    throw new Error('SDK token request failed');
  }

  const { idToken } = sdkTokenResponseSchema.parse(await sdkTokenResponse.json());
  return idToken;
};

const getUserInfo = async (
  idToken: string,
  url: URL,
  fetch: (url: URL, req: RequestInit) => Promise<Response>
) => {
  const userMeResponse = await fetch(new URL('/api/user/me', url.origin), {
    headers: {
      Authorization: `Bearer ${idToken}`
    }
  });

  if (userMeResponse.ok) {
    return userSchema.parse(await userMeResponse.json());
  } else if (userMeResponse.status === 404) {
    return null;
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

  if (!stateObject.originalPage || typeof stateObject.originalPage !== 'string') {
    return null;
  }

  return decodeURIComponent(stateObject.originalPage);
};
