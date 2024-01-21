import { SnackbarArgs, SnackbarToRender } from "./snackbarContext.ts";

export enum SnackbarActionKind {
  Add = "add",
}

export interface SnackbarAction {
  type: SnackbarActionKind;
  payload: SnackbarArgs;
}

export const snackbarReducer = (
  state: SnackbarToRender[],
  action: SnackbarAction,
) => {
  console.log({ state, action });
  if (action.type === SnackbarActionKind.Add) {
    const newSnackbar = { id: crypto.randomUUID(), ...action.payload };
    return [...state, newSnackbar];
  }

  return state;
};
