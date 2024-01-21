import { FC } from "react";
import { SnackbarContext } from "./snackbarContext.ts";
import TransientSnackbar from "./TransientSnackbar.tsx";

const SnackbarDisplay: FC = () => {
  return (
    <SnackbarContext.Consumer>
      {({ snackbarState }) =>
        snackbarState.map(({ props, children, id }) => (
          <TransientSnackbar key={id} {...props}>
            {children}
          </TransientSnackbar>
        ))
      }
    </SnackbarContext.Consumer>
  );
};

export default SnackbarDisplay;
