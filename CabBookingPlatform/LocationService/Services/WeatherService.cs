using System.Net.Http.Headers;
using System.Text.Json;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private const string Host = "weatherapi-com.p.rapidapi.com";

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetWeather(double lat, double lon)
    {
        string apiKey = _config["WeatherApi:ApiKey"];

        // Use query format expected by WeatherAPI (city name or lat,long combo)
        string location = $"{lat},{lon}";
        string url = $"https://{Host}/current.json?q={location}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-RapidAPI-Key", apiKey);
        request.Headers.Add("X-RapidAPI-Host", Host);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return $"{{\"error\":\"Weather API error: {response.StatusCode}\"}}";
        }

        return await response.Content.ReadAsStringAsync();
    }
}
