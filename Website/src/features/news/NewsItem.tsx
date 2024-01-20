import { FC, useEffect, useState } from "react";
import { Card, CardContent, Chip, Skeleton, Stack, Typography } from "@mui/joy";

const localStorageKey = (id: number) => `news-read-${id}`;

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
  const [isNew, setIsNew] = useState(false);

  useEffect(() => {
    const key = localStorageKey(id);
    const isRead = localStorage.getItem(key);

    if (!isRead) {
      setIsNew(true);
      localStorage.setItem(key, "true");
      return;
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
        <Typography width="100%">{description}</Typography>
      </CardContent>
    </Card>
  );
};

export default NewsItem;
