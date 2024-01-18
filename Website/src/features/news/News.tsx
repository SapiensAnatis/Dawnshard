import { FC } from "react";
import useNews from "./useNews.ts";
import { Box, List, Skeleton, Stack, Typography } from "@mui/joy";
import NewsItem, { SkeletonNewsItem } from "./NewsItem.tsx";

const News: FC = () => {
  const { data, isLoading } = useNews();

  const listItems = isLoading ? (
    <>
      <SkeletonNewsItem />
      <SkeletonNewsItem />
    </>
  ) : (
    data?.map(({ id, headline, description, time }) => (
      <NewsItem
        id={id}
        key={id}
        headline={headline}
        description={description}
        date={time}
      />
    ))
  );

  return (
    <Stack width="100%" height="100%" direction="column">
      <Box
        sx={{
          p: 2,
          backgroundColor: "background.surface",
          borderColor: "divider",
        }}
      >
        <Typography typography="h1">News and updates</Typography>
      </Box>
      <Box
        sx={{
          backgroundColor: "background.body",
          borderTop: "1px solid",
          borderColor: "divider",
          p: 2,
        }}
      >
        <List sx={{ display: "flex", gap: 2 }}>{listItems}</List>
      </Box>
    </Stack>
  );
};

export default News;
