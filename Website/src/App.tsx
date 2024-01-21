import "./App.css";
import router from "./Router.tsx";
import { RouterProvider } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import UserContext, { UserState } from "./shared/context/userContext.ts";
import { useReducer, useState } from "react";
import { snackbarReducer } from "./shared/snackbar/snackbarReducer.ts";
import { SnackbarContext } from "./shared/snackbar/snackbarContext.ts";
import SnackbarDisplay from "./shared/snackbar/SnackbarDisplay.tsx";

const queryClient = new QueryClient();

function App() {
  console.log("rendering app");

  const [userState, setUserState] = useState<UserState>({
    authenticated: false,
  });

  const [snackbarState, dispatchSnackbar] = useReducer(snackbarReducer, []);

  return (
    <SnackbarContext.Provider value={{ snackbarState, dispatchSnackbar }}>
      <UserContext.Provider value={{ userState, setUserState }}>
        <QueryClientProvider client={queryClient}>
          <RouterProvider router={router} />
          <ReactQueryDevtools
            initialIsOpen={false}
            buttonPosition={"bottom-left"}
          />
          <SnackbarDisplay />
        </QueryClientProvider>
      </UserContext.Provider>
    </SnackbarContext.Provider>
  );
}

export default App;
