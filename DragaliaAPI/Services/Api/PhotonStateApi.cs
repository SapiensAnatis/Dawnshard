using DragaliaAPI.Photon.Shared.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace DragaliaAPI.Services.Api;

public class PhotonStateApi : IPhotonStateApi
{
    private const string GameListEndpoint = "Get/GameList";
    private const string ByIdEndpoint = "Get/ById";
    private const string ByViewerIdEndpoint = "Get/ByViewerId";
    private const string IsHostEndpoint = "Get/IsHost";

    private readonly HttpClient client;

    public PhotonStateApi(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<ApiGame>> GetAllGames()
    {
        HttpResponseMessage response = await this.client.GetAsync(
            new Uri(GameListEndpoint, UriKind.Relative)
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<ApiGame>>()
            ?? Enumerable.Empty<ApiGame>();
    }

    public async Task<IEnumerable<ApiGame>> GetByQuestId(int questId)
    {
        Dictionary<string, string?> queryParams = new() { { nameof(questId), questId.ToString() } };
        Uri requestUri =
            new(QueryHelpers.AddQueryString(GameListEndpoint, queryParams), UriKind.Relative);

        HttpResponseMessage response = await this.client.GetAsync(requestUri);

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

    public async Task<bool> GetIsHost(long viewerId)
    {
        Uri endpoint = new($"{IsHostEndpoint}/{viewerId}", UriKind.Relative);

        HttpResponseMessage response = await this.client.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public async Task<ApiGame?> GetGameByViewerId(long viewerId)
    {
        Uri endpoint = new($"{ByViewerIdEndpoint}/{viewerId}", UriKind.Relative);

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
