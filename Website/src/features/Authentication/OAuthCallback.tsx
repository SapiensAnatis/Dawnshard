import { FC, useContext, useEffect } from "react";
import { doBaasAuthentication } from "./doBaasAuthentication.ts";
import { queryParams } from "../../shared/constants.ts";
import { Navigate, useSearchParams } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import UserContext from "../../shared/context/userContext.ts";
import { Typography } from "@mui/joy";
import { SnackbarContext } from "../../shared/snackbar/snackbarContext.ts";
import { SnackbarActionKind } from "../../shared/snackbar/snackbarReducer.ts";
import { ErrorRounded } from "@mui/icons-material";

const OAuthCallback: FC = () => {
  console.log("rendering oauth callback");

  const [searchParams] = useSearchParams();
  const { setUserState } = useContext(UserContext);
  const originalPage = searchParams.get(queryParams.originalPage) ?? "/";
  const sessionTokenCode = searchParams.get(queryParams.sessionTokenCode) ?? "";

  const { dispatchSnackbar } = useContext(SnackbarContext);

  const { mutate, isPending } = useMutation({
    mutationKey: ["baasLogin"],
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
    onError: (error) => {
      dispatchSnackbar({
        type: SnackbarActionKind.Add,
        payload: {
          props: {
            variant: "solid",
            color: "danger",
            startDecorator: <ErrorRounded />,
          },
          children: (
            <div>
              <Typography textColor="white">Authentication failure</Typography>
              <Typography fontSize="small">{error?.toString()}</Typography>
            </div>
          ),
        },
      });
    },
    retry: false,
  });

  useEffect(() => {
    console.log("mutating");
    mutate(sessionTokenCode);
  }, [mutate, sessionTokenCode]);

  if (!isPending) {
    console.log("redirecting to", originalPage);
    return <Navigate to={originalPage} />;
  }

  return <Typography>Redirecting...</Typography>;
};

export default OAuthCallback;
