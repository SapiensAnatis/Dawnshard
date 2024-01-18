import { useQuery } from "@tanstack/react-query";
import apiCall from "../../shared/apiCall.ts";

export type NewsItem = {
  id: number;
  headline: string;
  description: string;
  date: Date;
};

const useNews = () => {
  console.debug("usenews");
  return useQuery({
    queryKey: ["news"],
    queryFn: async () => await apiCall<NewsItem[]>("news"),
  });
};

export default useNews;
