import { localStorageKeys } from "../../shared/constants.ts";
import baasApiCall from "../../shared/baasApiCall.ts";
import apiCall from "../../shared/apiCall.ts";

type SessionTokenResponse = {
  session_token: string;
};

type SdkTokenResponse = {
  idToken: string;
};

type DawnshardAuthResponse = {
  viewerId: number;
  playerName: string;
};

export const doBaasAuthentication = async (sessionTokenCode: string) => {
  // Get session token
  const challengeString =
    localStorage.getItem(localStorageKeys.challengeString) ?? "";

  console.log({ challengeString });

  const sessionTokenForm = new FormData();
  sessionTokenForm.append("client_id", import.meta.env.VITE_BAAS_CLIENT_ID);
  sessionTokenForm.append("session_token_code", sessionTokenCode);
  sessionTokenForm.append("session_token_code_verifier", challengeString);

  const sessionTokenRequest = {
    body: sessionTokenForm,
    method: "POST",
  };

  const sessionTokenResponse = await baasApiCall<SessionTokenResponse>(
    "connect/1.0.0/api/session_token",
    sessionTokenRequest,
  );

  console.log({ sessionTokenResponse });

  const sdkTokenRequest = {
    method: "POST",
    body: JSON.stringify({
      client_id: import.meta.env.VITE_BAAS_CLIENT_ID,
      session_token: sessionTokenResponse.session_token,
    }),
    headers: {
      ["Content-Type"]: "application/json",
    },
  };

  const sdkTokenResponse = await baasApiCall<SdkTokenResponse>(
    "1.0.0/gateway/sdk/token",
    sdkTokenRequest,
  );

  console.log({ sdkTokenResponse });

  return await apiCall<DawnshardAuthResponse>("userDetails", {
    method: "GET",
    headers: {
      ["Authorization"]: `Bearer ${sdkTokenResponse.idToken}`,
    },
  });
};
