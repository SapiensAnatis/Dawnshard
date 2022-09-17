using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia
{
    public class ServiceStatusTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ServiceStatusTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task ServiceStatus_ReturnsCorrectJSON()
        {
            // Corresponds to JSON: "{}"
            byte[] payload = new byte[] { 0x80 };
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("tool/get_service_status", content);

            response.IsSuccessStatusCode.Should().BeTrue();
            byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
            string json = TestUtils.MsgpackBytesToPrettyJson(responseBytes);
            json.Should().BeEquivalentTo("""
                {
                  "data_headers": {
                    "result_code": 1
                  },
                  "data": {
                    "service_status": 1
                  }
                }
                """);
        }
    }
}
