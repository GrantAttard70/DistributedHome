// Services/WeatherService.cs
using System.Text.Json;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetWeather(double lat, double lon)
    {
        var url = $"{_config["WeatherApi:BaseUrl"]}?lat={lat}&lon={lon}&appid={_config["WeatherApi:ApiKey"]}&units=metric";
        var response = await _httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}
