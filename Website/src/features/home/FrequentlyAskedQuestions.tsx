import { FC, ReactNode } from "react";
import {
  Accordion,
  AccordionDetails,
  AccordionGroup,
  AccordionSummary,
  Box,
  List,
  ListItem,
  Typography,
} from "@mui/joy";

const FaqItem: FC<{ question: string; children: ReactNode }> = ({
  question,
  children,
}) => (
  <Accordion>
    <AccordionSummary>
      <Typography>{question}</Typography>
    </AccordionSummary>
    <AccordionDetails variant="soft">
      <Box sx={{ marginTop: "0.25rem" }}>{children}</Box>
    </AccordionDetails>
  </Accordion>
);

const FrequentlyAskedQuestions: FC = () => {
  return (
    <Box
      sx={{
        width: { xs: "100%", md: "75%" },
      }}
    >
      <Typography typography="h2" gutterBottom>
        Frequently asked questions
      </Typography>
      <AccordionGroup
        size="lg"
        variant="outlined"
        sx={{
          borderRadius: 8,
        }}
      >
        <FaqItem question="Can I link an account to save my progress across devices?">
          <Typography>
            {" "}
            Yes, if you follow the prompts to link an account in-game, you will
            be taken to the <a href="https://baas.lukefz.xyz">BaaS website</a>,
            also developed by LukeFZ. This is a drop-in replacement for the
            Nintendo account linking system from the original game, which uses
            entirely new credentials. If you create a new account and link it to
            your game&apos;s account, you will be able to access your progress
            from another device linked to the same account.
          </Typography>
        </FaqItem>
        <FaqItem question="Do I have to start over, or can I recover my progress?">
          <Typography>
            Unfortunately, it is no longer possible to recover your progress
            from the original servers. However, it is possible to import a save
            using the aforementioned account linking system. If you log in to a
            linked account, you can upload a JSON file with save data to be
            applied to the server. You can use a preset save file, such as this{" "}
            <a href="https://drive.google.com/drive/folders/17pR_hZtjIZ7NKBMUjtiY355FCM_-TqgO?usp=sharing">
              maxed out save file
            </a>
            , to skip parts of the game you do not want to play again.
          </Typography>
        </FaqItem>
        <FaqItem question="What features have been implemented so far?">
          <Typography>
            The majority of core gameplay mechanics have been implemented:
          </Typography>
          <List marker="disc" sx={{ marginLeft: "1rem" }}>
            <ListItem>Completing quests</ListItem>
            <ListItem>The Halidom castle builder</ListItem>
            <ListItem>The event compendium</ListItem>
            <ListItem>Co-op</ListItem>
            <ListItem>Kaleidoscape</ListItem>
            <ListItem>Upgrading weapons, wyrmprints, dragons, etc.</ListItem>
          </List>
        </FaqItem>
        <FaqItem question="What features are still being worked on?">
          <Typography>
            Here is a non-exhaustive list of features that are still being
            developed:
          </Typography>
          <List marker="disc" sx={{ marginLeft: "1rem" }}>
            <ListItem>Friends</ListItem>
            <ListItem>Alliances</ListItem>
            <ListItem>Campaign skip</ListItem>
            <ListItem>Alberian Battle Royale</ListItem>
            <ListItem>Astral raids</ListItem>
            <ListItem>
              Endeavours: some are available and are being slowly added
            </ListItem>
            <ListItem>
              Quest drops: all materials are obtainable, but work is ongoing to
              make the quantities accurate
            </ListItem>
            <ListItem>
              Summoning: only one banner is active with unrealistically high
              rates. All players get 1000 summons on starting.
            </ListItem>
          </List>
        </FaqItem>
      </AccordionGroup>
    </Box>
  );
};

export default FrequentlyAskedQuestions;
