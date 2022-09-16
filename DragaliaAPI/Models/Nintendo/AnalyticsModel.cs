namespace DragaliaAPI.Models.Nintendo
{
    public record AnalyticsConfigResponse
    {
        public string accessToken = "";
        public string applicationId = "";
        public string city = "" ;
        public string country = "" ;
        public long expirationTime = long.MaxValue;
        public bool immediateReporting = false;
        public string mode = "" ;
        public string region = "" ;
        public int reportingPeriod = int.MaxValue;
        public string topic = "";
    }
}
