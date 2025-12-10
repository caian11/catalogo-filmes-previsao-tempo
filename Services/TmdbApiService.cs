using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace catalogo_filmes_previsao_tempo.Services;

public class TmdbApiService : ITmdbApiService
{
    private readonly TMDbClient _client;
    private readonly IMemoryCache _cache;

    public TmdbApiService(IConfiguration configuration, IMemoryCache cache)
    {
        _cache = cache;

        var apiKey = configuration["TMDb:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("TMDb:ApiKey não configurada.");

        _client = new TMDbClient(apiKey);
    }

    public async Task<TMDbConfig> GetConfigurationAsync()
    {
        const string cacheKey = "tmdb_config";

        if (_cache.TryGetValue(cacheKey, out TMDbConfig cached))
            return cached;

        var config = await _client.GetConfigAsync();

        _cache.Set(
            cacheKey,
            config,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });

        return config;
    }
    
    public async Task<SearchContainer<SearchMovie>> SearchMoviesAsync(string query, int page = 1)
    {
        query ??= string.Empty;
        var normalizedQuery = query.Trim().ToLowerInvariant();

        var cacheKey = $"tmdb_search_{normalizedQuery}_p{page}";

        if (_cache.TryGetValue(cacheKey, out SearchContainer<SearchMovie> cached))
            return cached;

        var result = await _client.SearchMovieAsync(query, page: page);

        _cache.Set(
            cacheKey,
            result,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return result;
    }

    public async Task<Movie> GetMovieDetailsAsync(int tmdbId)
    {
        var cacheKey = $"tmdb_details_{tmdbId}";

        if (_cache.TryGetValue(cacheKey, out Movie cached))
            return cached;

        var movie = await _client.GetMovieAsync(
            tmdbId,
            MovieMethods.Credits | MovieMethods.Images);

        _cache.Set(
            cacheKey,
            movie,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return movie;
    }

    public async Task<ImagesWithId> GetMovieImagesAsync(int tmdbId)
    {
        var cacheKey = $"tmdb_images_{tmdbId}";

        if (_cache.TryGetValue(cacheKey, out ImagesWithId cached))
            return cached;

        var images = await _client.GetMovieImagesAsync(tmdbId);

        _cache.Set(
            cacheKey,
            images,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return images;
    }
}
