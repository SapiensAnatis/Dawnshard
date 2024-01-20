import { FC, useContext, useEffect } from "react";
import { doBaasAuthentication } from "./doBaasAuthentication.ts";
import { queryParams } from "../../shared/constants.ts";
import { Navigate, useSearchParams } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import UserContext from "../../userState.ts";
import { Typography } from "@mui/joy";

const OAuthCallback: FC = () => {
  console.log("rendering oauth callback");

  const [searchParams] = useSearchParams();
  const { setUserState } = useContext(UserContext);
  const originalPage = searchParams.get(queryParams.originalPage) ?? "/";
  const sessionTokenCode = searchParams.get(queryParams.sessionTokenCode) ?? "";

  const mutation = useMutation({
    mutationFn: async (tokenCode: string) =>
      await doBaasAuthentication(tokenCode),
    onSuccess: (response) => {
      console.log({ response });
      setUserState({
        authenticated: true,
        viewerId: response.viewerId,
        playerName: response.playerName,
      });
    },
    retry: false,
  });

  console.log(mutation);

  useEffect(() => {
    mutation.mutate(sessionTokenCode);
  }, [mutation, sessionTokenCode]);

  if (mutation.isError) {
    console.error(mutation.error);
    return (
      <>
        <Typography>Login failed</Typography>
        <Navigate to={"/news"} />
      </>
    );
  }

  return mutation.isPending ? (
    <Typography>Redirecting...</Typography>
  ) : (
    <Navigate to={originalPage} />
  );
};

export default OAuthCallback;
