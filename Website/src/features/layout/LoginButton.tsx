import { FC } from "react";
import { Button } from "@mui/joy";
import { Buffer } from "buffer";
import { queryParams } from "../../shared/constants.ts";

const getChallengeString = () => {
  const buffer = new Uint8Array(8);
  crypto.getRandomValues(buffer);
  return Array.from(buffer, (dec) => dec.toString(16).padStart(2, "0")).join(
    "",
  );
};

const getUrlSafeBase64Hash = async (input: string) => {
  const buffer = new TextEncoder().encode(input);
  const hashBuffer = await crypto.subtle.digest("SHA-256", buffer);
  const base64 = Buffer.from(new Uint8Array(hashBuffer)).toString("base64");
  return base64.replace("+", "-").replace("/", "_").replace("=", "");
};

const LoginButton: FC = () => {
  const handleClick = async () => {
    const challengeString = getChallengeString();

    localStorage.setItem("challenge-string", challengeString);
    const challengeStringHash = await getUrlSafeBase64Hash(challengeString);

    const params = new URLSearchParams({
      client_id: import.meta.env.VITE_BAAS_CLIENT_ID,
      redirect_uri: `${window.location.origin}/oauthcallback?${queryParams.originalPage}=${window.location.pathname}`,
      response_type: "session_token_code",
      scope: "user user.birthday openid",
      language: "en-US",
      session_token_code_challenge_method: "S256",
      session_token_code_challenge: challengeStringHash,
    });

    const url =
      new URL("custom/thirdparty/auth", import.meta.env.VITE_BAAS_URL) +
      "?" +
      params.toString();

    console.log(url);
    console.log(window.location);

    window.location.href = url;
  };

  return (
    <Button onClick={handleClick} variant="soft">
      Login
    </Button>
  );
};

export default LoginButton;
