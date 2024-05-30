export type JwtMetadata =
  | { valid: false }
  | {
      valid: true;
      expiryTimestampMs: number;
    };

const getJwtMetadata = (jwt: string): JwtMetadata => {
  const segments = jwt.split('.');
  if (segments.length < 3) {
    return { valid: false };
  }

  const payload = segments[1];
  let payloadObject;

  try {
    const decodedPayload = atob(payload);
    payloadObject = JSON.parse(decodedPayload);
  } catch {
    return { valid: false };
  }

  // We only care here if the token is expired. We are not validating signatures, etc. because that
  // can be left to the main API server. This is used to inform non-security-critical things like
  // whether to show the login button, or how long to store the JWT in a cookie for.
  const exp = payloadObject.exp;
  if (!exp || !Number.isInteger(exp)) {
    return { valid: false };
  }

  const expDate = new Date(exp * 1000);
  if (!Number.isInteger(expDate.valueOf())) {
    return { valid: false };
  }

  return {
    valid: true,
    expiry: expDate,
    expiryTimestampMs: exp * 1000
  };
};

export default getJwtMetadata;
