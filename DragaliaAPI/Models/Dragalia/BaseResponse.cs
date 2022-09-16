using Newtonsoft.Json;

namespace DragaliaAPI.Models.Dragalia
{
    public record BaseResponse
    {
        public DataHeaders data_headers;
        object? data;

        public BaseResponse(int resultCode)
            : this(new DataHeaders(resultCode))
        {
        }

        [JsonConstructor]
        public BaseResponse(DataHeaders dataHeaders)
        {
            this.data_headers = dataHeaders;
        }

        public record DataHeaders
        {
            public int result_code;

            public DataHeaders(int result_code)
            {
                this.result_code = result_code;
            }
        }
    }
}
