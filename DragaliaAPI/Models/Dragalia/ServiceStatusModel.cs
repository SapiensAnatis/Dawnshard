namespace DragaliaAPI.Models.Dragalia
{
    public record ServiceStatusResponse : BaseResponse
    {
        public int service_status = 1;

        public ServiceStatusResponse()
            : base(1)
        { 
        }
    }
}
