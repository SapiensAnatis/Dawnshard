import React, { SetStateAction } from "react";

export type UserState =
  | {
      authenticated: false;
      viewerId?: null;
      playerName?: null;
    }
  | {
      authenticated: true;
      viewerId: number;
      playerName: string;
    };

export type UserContextType = {
  userState: UserState;
  setUserState: React.Dispatch<SetStateAction<UserState>>;
};

const UserContext = React.createContext<UserContextType>({
  userState: { authenticated: false },
  setUserState: () => {},
});

export default UserContext;
