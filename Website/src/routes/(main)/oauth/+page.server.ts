import type { PageServerLoad } from './$types';
import { PUBLIC_BAAS_URL, PUBLIC_BAAS_CLIENT_ID } from '$env/static/public';
import Cookies from '$lib/auth/cookies.ts';
import { redirect } from '@sveltejs/kit';
import getJwtMetadata from '$lib/auth/jwt.ts';

const sessionTokenUrl = new URL('/connect/1.0.0/api/session_token', PUBLIC_BAAS_URL);
const sdkTokenUrl = new URL('/1.0.0/gateway/sdk/token', PUBLIC_BAAS_URL);

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

export const load: PageServerLoad = async ({ cookies, url, fetch }) => {
  const challengeString = cookies.get(Cookies.ChallengeString);

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

  const sessionTokenResponseBody = await sessionTokenResponse.json();
  const sessionToken = sessionTokenResponseBody.session_token;

  if (!sessionToken) {
    throw new Error('Failed to parse session token response');
  }

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

  const sdkTokenResponseBody = await sdkTokenResponse.json();
  const idToken = sdkTokenResponseBody.idToken;

  if (!idToken) {
    throw new Error('Failed to parse SDK token response');
  }

  const jwtMetadata = getJwtMetadata(idToken);
  if (!jwtMetadata.valid) {
    throw Error('Invalid JWT returned');
  }

  console.log(jwtMetadata);

  const maxAge = (jwtMetadata.expiryTimestampMs - Date.now()) / 1000;

  cookies.set(Cookies.IdToken, idToken, {
    path: '/',
    sameSite: 'lax',
    httpOnly: true,
    maxAge,
    ...(import.meta.env.MODE !== 'development' && {
      secure: true
    })
  });

  cookies.delete('challengeString', {
    path: '/'
  });

  const destination = getOriginalPage(url) ?? '/';
  redirect(302, destination);
};
