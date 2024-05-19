import type { PageServerLoad } from './$types';
import { PUBLIC_BAAS_URL, PUBLIC_BAAS_CLIENT_ID } from '$env/static/public';

export const load: PageServerLoad = async ({ cookies, url }) => {
  const challengeString = cookies.get('challengeString');

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
  const sessionTokenResponse = await fetch(`${PUBLIC_BAAS_URL}connect/1.0.0/api/session_token`, {
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
  const sdkTokenResponse = await fetch(`${PUBLIC_BAAS_URL}1.0.0/gateway/sdk/token`, {
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

  cookies.set('idToken', idToken, {
    path: '/',
    sameSite: 'strict',
    httpOnly: true,
    ...(import.meta.env.MODE !== 'development' && {
      secure: true
    })
  });
};
