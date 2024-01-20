import { createBrowserRouter, redirect, RouteObject } from "react-router-dom";
import Root from "./Root.tsx";
import { pages } from "./Pages.tsx";
import { Typography } from "@mui/joy";
import { queryParams } from "./shared/constants.ts";
import { doBaasAuthentication } from "./features/Authentication/doBaasAuthentication.ts";
import OAuthCallback from "./features/Authentication/OAuthCallback.tsx";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: pages.map(({ path, element }) => ({
      path: path,
      element: element,
    })),
  },
  {
    path: "/oauthcallback",
    element: <OAuthCallback />,
  },
]);

export default router;
