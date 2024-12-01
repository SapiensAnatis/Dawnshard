using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Integration.Test.Features.Web;

public class WebTestFixture : TestFixture
{
    /// <summary>
    /// The web account ID.
    /// </summary>
    protected string WebAccountId { get; } = $"web_account_id_{Guid.NewGuid()}";

    protected WebTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

    protected void SetupMockBaas()
    {
        this.MockBaasApi.Setup(x =>
                x.GetUserId(It.Is<string>(token => GetSubject(token) == WebAccountId))
            )
            .ReturnsAsync(DeviceAccountId);
    }

    protected void AddTokenCookie()
    {
        string token = TokenHelper.GetToken(
            WebAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );

        this.Client.DefaultRequestHeaders.Add("Cookie", $"idToken={token}");
    }

    private static string GetSubject(string token)
    {
        string[] segments = token.Split('.');
        string dataJson = Base64UrlEncoder.Decode(segments[1]);
        JsonDocument document = JsonDocument.Parse(dataJson);
        return document.RootElement.GetProperty("sub").GetString()
            ?? throw new InvalidOperationException("Failed to parse token");
    }
}
