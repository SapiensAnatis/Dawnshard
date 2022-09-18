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
            // Corresponds to JSON: "{}"
            byte[] payload = new byte[] { 0x80 };
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version_list", content);

            response.IsSuccessStatusCode.Should().BeTrue();

            byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
            var deserializedResponse = MessagePackSerializer.Deserialize<EulaGetVersionListResponse>(responseBytes, ContractlessStandardResolver.Options);

            deserializedResponse.data_headers.result_code.Should().Be(ResultCode.Success);
            deserializedResponse.data.version_hash_list.Should().BeEquivalentTo(EulaData.AllEulaVersions);
        }

        [Fact]
        public async Task EulaGetVersion_ValidRegionAndLocale_ReturnsEulaData()
        {
            EulaVersion expectedVersion = new("gb", "en_eu", 1, 1);
            var data = new { region = "gb", lang = "en_eu" };
            byte[] payload = MessagePackSerializer.Serialize(data);
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version", content);

            response.IsSuccessStatusCode.Should().BeTrue();

            byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
            var deserializedResponse = MessagePackSerializer.Deserialize<EulaGetVersionResponse>(responseBytes, ContractlessStandardResolver.Options);

            deserializedResponse.data_headers.result_code.Should().Be(ResultCode.Success);
            deserializedResponse.data.version_hash.Should().BeEquivalentTo(expectedVersion);
            deserializedResponse.data.agreement_status.Should().Be(0);
            deserializedResponse.data.is_required_agree.Should().BeFalse();
        }

        [Fact]
        public async Task EulaGetVersion_InvalidRegionOrLocale_ReturnsBadRequest()
        {
            var data = new { region = "microsoft", lang = "en_c#" };
            byte[] payload = MessagePackSerializer.Serialize(data);
            HttpContent content = TestUtils.CreateMsgpackContent(payload);

            var response = await _client.PostAsync("eula/get_version", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
