import { SnackbarProps, Snackbar, IconButton } from "@mui/joy";
import { FC, useState } from "react";
import { CloseRounded } from "@mui/icons-material";

const TransientSnackbar: FC<Omit<SnackbarProps, "open">> = ({
  onClose,
  autoHideDuration,
  ...props
}) => {
  const [open, setOpen] = useState(true);

  const durationOrDefault = autoHideDuration ?? 3000;
  console.log({ durationOrDefault });

  return (
    <Snackbar
      {...props}
      invertedColors
      open={open}
      onClose={(event, reason) => {
        if (onClose) onClose(event, reason);
        setOpen(false);
      }}
      endDecorator={
        <IconButton component={CloseRounded} onClick={() => setOpen(false)} />
      }
      autoHideDuration={durationOrDefault}
    />
  );
};

export default TransientSnackbar;
