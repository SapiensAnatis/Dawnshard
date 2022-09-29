using DragaliaAPI.Models.Dragalia.Responses;
using MessagePack;
using MessagePack.Resolvers;

namespace DragaliaAPI.Test.Integration.Dragalia
{
    public class EulaTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public EulaTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task EulaGetVersionList_ReturnsAllVersions()
        {
            EulaGetVersionListResponse expectedResponse = new(); // Constructor automatically populates with all versions

            // Corresponds to JSON: "{}"
            byte[] payload = new byte[] { 0x80 };
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version_list", content);

            response.IsSuccessStatusCode.Should().BeTrue();

            await TestUtils.CheckMsgpackResponse(response, expectedResponse);
        }

        [Fact]
        public async Task EulaGetVersion_ValidRegionAndLocale_ReturnsEulaData()
        {
            EulaGetVersionData expectedData = new(new EulaVersion("gb", "en_eu", 1, 1));
            EulaGetVersionResponse expectedResponse = new(expectedData);

            var data = new { region = "gb", lang = "en_eu" };
            byte[] payload = MessagePackSerializer.Serialize(data);
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version", content);

            await TestUtils.CheckMsgpackResponse(response, expectedResponse);
        }

        [Fact]
        public async Task EulaGetVersion_InvalidRegionOrLocale_ReturnsDefault()
        {
            EulaGetVersionData expectedData = new(new EulaVersion("gb", "en_us", 1, 1));
            EulaGetVersionResponse expectedResponse = new(expectedData);

            var data = new { region = "microsoft", lang = "en_c#" };
            byte[] payload = MessagePackSerializer.Serialize(data);
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version", content);

            await TestUtils.CheckMsgpackResponse(response, expectedResponse);
        }
    }
}
