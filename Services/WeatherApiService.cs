using System.Globalization;

namespace catalogo_filmes_previsao_tempo.Services;

using System.Net.Http;
using System.Text.Json;
using catalogo_filmes_previsao_tempo.Services;
using catalogo_filmes_previsao_tempo.Models;

public class WeatherApiService : IWeatherApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherApiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<WeatherForecastDto?> GetWeatherAsync(double latitude, double longitude)
    {
        var baseUrl = _config["OpenMeteo:BaseUrl"];

        var url = $"{baseUrl}forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
                  $"&daily=temperature_2m_max,temperature_2m_min&timezone=auto";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<OpenMeteoResponse>(json);

        if (data?.daily == null)
            return null;

        return new WeatherForecastDto
        {
            Date = data.daily.time[0],
            MinTemperature = data.daily.temperature_2m_min[0],
            MaxTemperature = data.daily.temperature_2m_max[0]
        };
    }
}
