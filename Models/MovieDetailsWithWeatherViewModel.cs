namespace catalogo_filmes_previsao_tempo.Models
{
    public class MovieDetailsWithWeatherViewModel
    {
        public Filme Filme { get; set; }
        public WeatherForecastDto? Weather { get; set; }
        public string? WeatherError { get; set; }
    }
}