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

const parseResponse = ({ time, ...props }: NewsResponse): NewsItem => {
  return {
    time: new Date(time * 1000),
    ...props,
  };
};

const useNews = () => {
  console.debug("usenews");
  return useQuery({
    queryKey: ["news"],
    queryFn: async () =>
      (await apiCall<NewsResponse[]>("news"))
        .sort((a, b) => b.time - a.time)
        .map(parseResponse),
  });
};

export default useNews;
