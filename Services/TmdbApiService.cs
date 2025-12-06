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

    public TmdbApiService(IConfiguration configuration)
    {
        var apiKey = configuration["TMDb:ApiKey"];
        _client = new TMDbClient(apiKey);
    }

    public Task<SearchContainer<SearchMovie>> SearchMoviesAsync(string query, int page = 1)
    {
        return _client.SearchMovieAsync(query, page: page);
    }

    public Task<Movie> GetMovieDetailsAsync(int tmdbId)
    {
        return _client.GetMovieAsync(tmdbId, MovieMethods.Credits | MovieMethods.Images);
    }

    public Task<ImagesWithId> GetMovieImagesAsync(int tmdbId)
    {
        return _client.GetMovieImagesAsync(tmdbId);
    }

    public Task<TMDbConfig> GetConfigurationAsync()
    {
        return _client.GetConfigAsync();
    }
}