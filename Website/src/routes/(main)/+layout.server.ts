import type { LayoutServerLoad } from './$types';
import getJwtMetadata from '$lib/jwt';
import Cookies from '$lib/cookies';

export const load: LayoutServerLoad = ({ cookies, depends }) => {
  depends(`cookie:${Cookies.IdToken}`);

  const idToken = cookies.get(Cookies.IdToken);
  if (!idToken) {
    return {
      hasValidJwt: false
    };
  }

  const jwtMetadata = getJwtMetadata(idToken);
  if (!jwtMetadata.valid || Date.now() > jwtMetadata.expiryTimestampMs) {
    return {
      hasValidJwt: false
    };
  }

  return {
    hasValidJwt: true
  };
};
