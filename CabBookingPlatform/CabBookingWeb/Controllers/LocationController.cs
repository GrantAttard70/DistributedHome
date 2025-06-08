using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

[Authorize]
public class LocationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LocationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.GetAsync("/api/locations");

        if (response.IsSuccessStatusCode)
        {
            var locations = await response.Content.ReadFromJsonAsync<List<LocationViewModel>>();
            return View(locations);
        }

        TempData["ErrorMessage"] = "Could not load locations.";
        return View(new List<LocationViewModel>());
    }

    private string GetUserToken()
    {
        return User.FindFirst("Token")?.Value;
    }
}
