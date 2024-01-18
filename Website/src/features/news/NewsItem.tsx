import { FC, useEffect, useState } from "react";
import { Card, CardContent, Chip, Skeleton, Stack, Typography } from "@mui/joy";

const maxDescriptionLength = 300;
const localStorageKey = "news-latest-read";

export const SkeletonNewsItem: FC = () => {
  return (
    <Card variant="outlined" orientation="horizontal" component="li" key={1}>
      <CardContent>
        <Stack direction="row" gap={2} alignItems="center">
          <Typography typography="h2">
            <Skeleton>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit
            </Skeleton>
          </Typography>
        </Stack>
        <Typography fontSize="small">
          <Skeleton>Sample date</Skeleton>
        </Typography>
        <Typography width="100%">
          <Skeleton>
            Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
          </Skeleton>
        </Typography>
      </CardContent>
    </Card>
  );
};

const NewsItem: FC<{
  id: number;
  headline: string;
  description: string;
  date: Date;
}> = ({ id, headline, description, date }) => {
  const trimmedDescription =
    description.length > maxDescriptionLength
      ? description.substring(0, maxDescriptionLength - 1) + "…"
      : description;

  const [isNew, setIsNew] = useState(false);

  useEffect(() => {
    const latestReadStr = localStorage.getItem(localStorageKey);

    if (!latestReadStr) {
      setIsNew(true);
      localStorage.setItem(localStorageKey, id.toString());
      return;
    }

    const latestRead = parseInt(latestReadStr);

    if (id > latestRead) {
      setIsNew(true);
      localStorage.setItem(localStorageKey, id.toString());
    }
  }, [id]);

  return (
    <Card
      variant="outlined"
      orientation="horizontal"
      component="li"
      sx={{
        "&:hover": {
          boxShadow: "lg",
          borderColor: "var(--joy-palette-neutral-outlinedDisabledBorder)",
        },
      }}
    >
      <CardContent>
        <Stack direction="row" gap={2} alignItems="center">
          <Typography typography="h2">{headline}</Typography>
          {isNew && (
            <Chip color="primary" variant="solid" sx={{ height: "1rem" }}>
              New
            </Chip>
          )}
        </Stack>
        <Typography fontSize="small">{date.toLocaleString()}</Typography>
        <Typography width="100%">{trimmedDescription}</Typography>
      </CardContent>
    </Card>
  );
};

export default NewsItem;
