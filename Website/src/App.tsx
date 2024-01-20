import "./App.css";
import router from "./Router.tsx";
import { RouterProvider } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import UserContext, { UserState } from "./userState.ts";
import { useState } from "react";

const queryClient = new QueryClient();

function App() {
  const [userState, setUserState] = useState<UserState>({
    authenticated: false,
  });

  return (
    <UserContext.Provider value={{ userState, setUserState }}>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </UserContext.Provider>
  );
}

export default App;
