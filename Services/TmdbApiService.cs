using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace catalogo_filmes_previsao_tempo.Services;

public class TmdbApiService : ITmdbApiService
{
    private readonly TMDbClient _client;
    private readonly ILogger<TmdbApiService> _logger;

    public TmdbApiService(IConfiguration configuration, ILogger<TmdbApiService> logger)
    {
        var apiKey = configuration["TMDb:ApiKey"];
        _client = new TMDbClient(apiKey);
        _logger = logger;
        _logger.LogInformation("TMDbApiService inicializado");
    }

    public async Task<SearchContainer<SearchMovie>> SearchMoviesAsync(string query, int page = 1)
    {
        try
        {
          return await _client.SearchMovieAsync(query, page: page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar filmes no TMDb. Query='{query}'", query);
            throw;
        }
    }

    public async Task<Movie> GetMovieDetailsAsync(int tmdbId)
    {
        try
        {
            var movie = await _client.GetMovieAsync(tmdbId, MovieMethods.Credits | MovieMethods.Images);

            if (movie == null) _logger.LogWarning("TMDb não encontrou detalhes para o ID={tmdbId}", tmdbId);

            return movie;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes do filme. ID={tmdbId}", tmdbId);
            throw;
        }
    }

    public async Task<ImagesWithId> GetMovieImagesAsync(int tmdbId)
    {
        try
        {
            return await _client.GetMovieImagesAsync(tmdbId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar imagens do filme. ID={tmdbId}", tmdbId);
            throw;
        }
    }

    public async Task<TMDbConfig> GetConfigurationAsync()
    {

        try
        {
            return await _client.GetConfigAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar configurações do TMDb");
            throw;
        }
    }
}