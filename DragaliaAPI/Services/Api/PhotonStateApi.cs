using DragaliaAPI.Photon.Dto;

namespace DragaliaAPI.Services.Api;

public class PhotonStateApi : IPhotonStateApi
{
    private static readonly Uri GameListEndpoint = new("/get/gamelist", UriKind.Relative);
    private readonly HttpClient client;

    public PhotonStateApi(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<StoredGame>> GetAllGames()
    {
        HttpResponseMessage response = await this.client.GetAsync(GameListEndpoint);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<StoredGame>>()
            ?? Enumerable.Empty<StoredGame>();
    }
}
