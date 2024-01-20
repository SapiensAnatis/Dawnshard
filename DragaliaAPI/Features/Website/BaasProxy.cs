using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Website;

[Route("web/[controller]")]
[AllowAnonymous]
public class BaasController : ControllerBase
{
    private readonly HttpClient client;
    private readonly ILogger<BaasController> logger;

    public BaasController(
        HttpClient client,
        IOptions<BaasOptions> baasOptions,
        ILogger<BaasController> logger
    )
    {
        this.client = client;
        this.logger = logger;
        this.client.BaseAddress = baasOptions.Value.BaasUrlParsed;
    }

    [HttpPost("{*baasPath}")]
    public async Task<IActionResult> ProxyBaasRequest(string baasPath)
    {
        HttpRequestMessage forwardRequest = new HttpRequestMessage(HttpMethod.Post, baasPath);
        forwardRequest.Content = new StreamContent(this.Request.Body);

        this.logger.LogInformation("Sending request {ForwardRequest}", forwardRequest);

        HttpResponseMessage responseMessage = await this.client.SendAsync(forwardRequest);

        this.logger.LogInformation("Got response {Response}", responseMessage);

        if (!responseMessage.IsSuccessStatusCode)
            return this.StatusCode((int)responseMessage.StatusCode);

        object? response = await responseMessage.Content.ReadFromJsonAsync<object>();

        this.logger.LogInformation("Got response body {Response}", response);

        return this.StatusCode((int)responseMessage.StatusCode, response);
    }
}
