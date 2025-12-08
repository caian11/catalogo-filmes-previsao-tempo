using catalogo_filmes_previsao_tempo.Models;

namespace catalogo_filmes_previsao_tempo.Services;

public interface IWeatherApiService
{
    Task<WeatherForecastDto?> GetWeatherAsync(double latitude, double longitude);
}
