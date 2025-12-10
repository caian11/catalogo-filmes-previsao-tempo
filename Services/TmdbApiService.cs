using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
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
    private readonly IMemoryCache _cache;

    private const string ConfigCacheKey = "Tmdb:Config";

    public TmdbApiService(
        IConfiguration configuration,
        ILogger<TmdbApiService> logger,
        IMemoryCache cache)
    {
        var apiKey = configuration["TMDb:ApiKey"]
                     ?? throw new ArgumentNullException("TMDb:ApiKey não configurada");

        _client = new TMDbClient(apiKey);
        _logger = logger;
        _cache = cache;

        _logger.LogInformation("TMDbApiService inicializado");
    }

    public async Task<SearchContainer<SearchMovie>> SearchMoviesAsync(string query, int page = 1)
    {
        var cacheKey = $"Tmdb:Search:{query}:{page}";

        if (_cache.TryGetValue<SearchContainer<SearchMovie>>(cacheKey, out var cachedResult))
            return cachedResult;

        try
        {
            var result = await _client.SearchMovieAsync(query, page: page);

            _cache.Set(
                cacheKey,
                result,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) 
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar filmes no TMDb. Query='{Query}', Page={Page}", query, page);
            throw;
        }
    }

    public async Task<Movie> GetMovieDetailsAsync(int tmdbId)
    {
        var cacheKey = $"Tmdb:MovieDetails:{tmdbId}";

        if (_cache.TryGetValue<Movie>(cacheKey, out var cachedMovie))
            return cachedMovie;

        try
        {
            var movie = await _client.GetMovieAsync(tmdbId, MovieMethods.Credits | MovieMethods.Images);

            if (movie == null)
            {
                _logger.LogWarning("TMDb não encontrou detalhes para o ID={TmdbId}", tmdbId);
                return null;
            }

            _cache.Set(
                cacheKey,
                movie,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) 
            );

            return movie;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes do filme. ID={TmdbId}", tmdbId);
            throw;
        }
    }

    public async Task<ImagesWithId> GetMovieImagesAsync(int tmdbId)
    {
        var cacheKey = $"Tmdb:MovieImages:{tmdbId}";

        if (_cache.TryGetValue<ImagesWithId>(cacheKey, out var cachedImages))
            return cachedImages;

        try
        {
            var images = await _client.GetMovieImagesAsync(tmdbId);

            _cache.Set(
                cacheKey,
                images,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            );

            return images;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar imagens do filme. ID={TmdbId}", tmdbId);
            throw;
        }
    }

    public async Task<TMDbConfig> GetConfigurationAsync()
    {
        if (_cache.TryGetValue<TMDbConfig>(ConfigCacheKey, out var cachedConfig))
            return cachedConfig;

        try
        {
            var config = await _client.GetConfigAsync();
            
            _cache.Set(
                ConfigCacheKey,
                config,
                new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12))
            );

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar configurações do TMDb");
            throw;
        }
    }
}
