import { useQuery } from "@tanstack/react-query";
import apiCall from "../../shared/apiCall.ts";

export type NewsResponse = {
  id: number;
  headline: string;
  description: string;
  time: number;
};

export type NewsItem = {
  id: number;
  headline: string;
  description: string;
  time: Date;
};

const parseResponse = ({
  id,
  headline,
  description,
  time,
}: NewsResponse): NewsItem => {
  return {
    id: id,
    headline: headline,
    description: description,
    time: new Date(time * 1000),
  };
};

const useNews = () => {
  console.debug("usenews");
  return useQuery({
    queryKey: ["news"],
    queryFn: async () =>
      (await apiCall<NewsResponse[]>("news")).map(parseResponse),
  });
};

export default useNews;
