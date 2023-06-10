using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Services.Api;

public class PhotonStateApi : IPhotonStateApi
{
    private static readonly Uri GameListEndpoint = new("/get/gamelist", UriKind.Relative);
    private const string ByIdEndpoint = "/get/byid";

    private readonly HttpClient client;

    public PhotonStateApi(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<ApiGame>> GetAllGames()
    {
        HttpResponseMessage response = await this.client.GetAsync(GameListEndpoint);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<ApiGame>>()
            ?? Enumerable.Empty<ApiGame>();
    }

    public async Task<ApiGame?> GetGameById(int id)
    {
        Uri endpoint = new($"{ByIdEndpoint}/{id}", UriKind.Relative);

        HttpResponseMessage response = await this.client.GetAsync(endpoint);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode is System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ApiGame>();
    }
}
