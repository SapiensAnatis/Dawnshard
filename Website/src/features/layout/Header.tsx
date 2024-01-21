import { FC, useContext } from "react";
import { Box, IconButton, Stack, Typography } from "@mui/joy";
import ColorSchemeToggle from "./ColorSchemeToggle.tsx";
import { MenuRounded } from "@mui/icons-material";
import useBreakpoint from "../../shared/hooks/useBreakpoint.ts";
import UserContext from "../../shared/context/userContext.ts";
import LoginButton from "./LoginButton.tsx";

const Header: FC<{ onClickMenu: () => void }> = ({ onClickMenu }) => {
  const small = useBreakpoint((breakpoints) => breakpoints.down("sm"));
  const { userState } = useContext(UserContext);

  return (
    <Box
      sx={{
        display: "flex",
        flexGrow: 1,
        justifyContent: "space-between",
      }}
    >
      <Stack
        direction="row"
        justifyContent="space-between"
        alignItems="center"
        gap={2}
        flexGrow={1}
      >
        {small && (
          <IconButton
            component={MenuRounded}
            onClick={onClickMenu}
          ></IconButton>
        )}
        <Typography level="h2" sx={{ alignSelf: "left" }}>
          Dawnshard
        </Typography>
        <Box sx={{ flexGrow: 1 }} />(
        {userState.authenticated ? (
          <Typography>Logged in as {userState.playerName}</Typography>
        ) : (
          <LoginButton />
        )}
        <ColorSchemeToggle />
      </Stack>
    </Box>
  );
};

export default Header;
