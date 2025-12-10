using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using catalogo_filmes_previsao_tempo.Models;

namespace catalogo_filmes_previsao_tempo.Services;

public class WeatherApiService : IWeatherApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<WeatherApiService> _logger;

    public WeatherApiService(HttpClient httpClient, IConfiguration config, ILogger<WeatherApiService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<WeatherForecastDto?> GetWeatherAsync(double latitude, double longitude)
    {
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

        _logger.LogInformation(
            "Previsão obtida com sucesso | Min: {min} | Max: {max} | Data: {data}",
            data.daily.temperature_2m_min[0],
            data.daily.temperature_2m_max[0],
            data.daily.time[0]
        );

        return new WeatherForecastDto
        {
            Date = data.daily.time[0],
            MinTemperature = data.daily.temperature_2m_min[0],
            MaxTemperature = data.daily.temperature_2m_max[0]
        };
    }
}
