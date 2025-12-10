using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using catalogo_filmes_previsao_tempo.Models;
using Microsoft.Extensions.Caching.Memory;

namespace catalogo_filmes_previsao_tempo.Services;

public class WeatherApiService : IWeatherApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<WeatherApiService> _logger;
    private readonly IMemoryCache _cache;

    public WeatherApiService(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<WeatherApiService> logger,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
        _cache = cache;
    }

    public async Task<WeatherForecastDto?> GetWeatherAsync(double latitude, double longitude)
    {
        var cacheKey = $"Weather:{latitude.ToString(CultureInfo.InvariantCulture)}:{longitude.ToString(CultureInfo.InvariantCulture)}";

        if (_cache.TryGetValue<WeatherForecastDto>(cacheKey, out var cachedForecast))
        {
            _logger.LogInformation(
                "Previsão retornada do cache | Latitude: {lat} | Longitude: {lon}",
                latitude, longitude
            );

            return cachedForecast;
        }

        var baseUrl = _config["OpenMeteo:BaseUrl"];

        var url =
            $"{baseUrl}forecast?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
            $"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
            $"&daily=temperature_2m_max,temperature_2m_min&timezone=auto";

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao tentar conectar com Open-Meteo | Latitude: {lat} | Longitude: {lon}",
                latitude, longitude
            );

            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Erro na API Open-Meteo | StatusCode: {status} | Latitude: {lat} | Longitude: {lon}",
                response.StatusCode, latitude, longitude
            );

            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        OpenMeteoResponse? data;

        try
        {
            data = JsonSerializer.Deserialize<OpenMeteoResponse>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao desserializar JSON da Open-Meteo | JSON recebido: {json}",
                json
            );

            return null;
        }

        if (data?.daily == null)
        {
            _logger.LogWarning(
                "Resposta Open-Meteo não contém dados de 'daily' | JSON: {json}",
                json
            );

            return null;
        }

        var forecast = new WeatherForecastDto
        {
            Date = data.daily.time[0],
            MinTemperature = data.daily.temperature_2m_min[0],
            MaxTemperature = data.daily.temperature_2m_max[0]
        };

        _logger.LogInformation(
            "Previsão obtida com sucesso | Min: {min} | Max: {max} | Data: {data}",
            forecast.MinTemperature,
            forecast.MaxTemperature,
            forecast.Date
        );
        
        _cache.Set(
            cacheKey,
            forecast,
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
        );

        return forecast;
    }
}
