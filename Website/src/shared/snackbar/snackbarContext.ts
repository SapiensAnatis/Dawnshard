import { SnackbarProps } from "@mui/joy";
import { createContext, ReactNode } from "react";
import { SnackbarAction } from "./snackbarReducer.ts";

export type SnackbarArgs = {
  props: Omit<SnackbarProps, "open">;
  children: ReactNode;
};

export type SnackbarToRender = SnackbarArgs & { id: string };

export type SnackbarContextType = {
  snackbarState: SnackbarToRender[];
  dispatchSnackbar: (action: SnackbarAction) => void;
};

export const SnackbarContext = createContext<SnackbarContextType>({
  snackbarState: [],
  dispatchSnackbar: () => {},
});
