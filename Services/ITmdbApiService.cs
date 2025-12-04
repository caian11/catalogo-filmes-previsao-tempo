using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace catalogo_filmes_previsao_tempo.Services;

public interface ITmdbApiService
{
    Task<SearchContainer<SearchMovie>> SearchMoviesAsync(string query, int page = 1);
    Task<Movie> GetMovieDetailsAsync(int tmdbId);
    Task<ImagesWithId> GetMovieImagesAsync(int tmdbId);
    Task<TMDbConfig> GetConfigurationAsync();
}