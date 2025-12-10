using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using catalogo_filmes_previsao_tempo.Data;
using catalogo_filmes_previsao_tempo.Models;
using catalogo_filmes_previsao_tempo.Services;

namespace catalogo_filmes_previsao_tempo.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ITmdbApiService _tmdb;
        private readonly IFilmeRepository _filmes;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(
            ITmdbApiService tmdb,
            IFilmeRepository filmes,
            ILogger<MoviesController> logger)
        {
            _tmdb = tmdb;
            _filmes = filmes;
            _logger = logger;
        }

        // GET /Movies/Search?query=...&page=1
        public async Task<IActionResult> Search(string? query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Busca feita sem query.");
                return View(new MovieSearchViewModel { Query = "", Page = 1, TotalPages = 0 });
            }

            try
            {
                var searchResult = await _tmdb.SearchMoviesAsync(query, page);
                var config = await _tmdb.GetConfigurationAsync();

                var baseUrl = config.Images.SecureBaseUrl ?? config.Images.BaseUrl;
                var size = config.Images.PosterSizes.Contains("w342")
                    ? "w342"
                    : config.Images.PosterSizes.Last();

                var vm = new MovieSearchViewModel
                {
                    Query = query,
                    Page = searchResult.Page,
                    TotalPages = searchResult.TotalPages,
                    Results = searchResult.Results.Select(r => new MovieListItemViewModel
                    {
                        TmdbId = r.Id,
                        Titulo = r.Title,
                        Sinopse = r.Overview,
                        DataLancamento = r.ReleaseDate,
                        PosterUrl = string.IsNullOrEmpty(r.PosterPath)
                            ? null
                            : $"{baseUrl}{size}{r.PosterPath}"
                    }).ToList()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar filmes na API do TMDb. Query={Query}, Page={Page}", query, page);
                return BadRequest("Erro ao buscar filmes.");
            }
        }

        // GET /Movies/Details?tmdbId=123
        public async Task<IActionResult> Details(int tmdbId)
        {
            try
            {
                var movie = await _tmdb.GetMovieDetailsAsync(tmdbId);
                var config = await _tmdb.GetConfigurationAsync();

                var baseUrl = config.Images.SecureBaseUrl ?? config.Images.BaseUrl;
                var size = config.Images.PosterSizes.Contains("w342")
                    ? "w342"
                    : config.Images.PosterSizes.Last();

                var elenco = movie.Credits?.Cast?
                    .OrderBy(c => c.Order)
                    .Take(5)
                    .Select(c => c.Name);

                var vm = new MovieDetailsViewModel
                {
                    TmdbId = movie.Id,
                    Titulo = movie.Title,
                    TituloOriginal = movie.OriginalTitle,
                    Sinopse = movie.Overview,
                    DataLancamento = movie.ReleaseDate,
                    Genero = movie.Genres != null ? string.Join(", ", movie.Genres.Select(g => g.Name)) : null,
                    PosterUrl = string.IsNullOrEmpty(movie.PosterPath)
                        ? null
                        : $"{baseUrl}{size}{movie.PosterPath}",
                    Lingua = movie.OriginalLanguage,
                    Duracao = movie.Runtime,
                    NotaMedia = movie.VoteAverage,
                    ElencoPrincipal = elenco != null ? string.Join(", ", elenco) : null
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do filme TMDbId={Id}", tmdbId);
                return BadRequest("Erro ao carregar detalhes.");
            }
        }

        // GET /Movies/Import?tmdbId=123 
        [HttpGet]
        public async Task<IActionResult> Import(int tmdbId)
        {
            try
            {
                var movie = await _tmdb.GetMovieDetailsAsync(tmdbId);

                var elenco = movie.Credits?.Cast?
                    .OrderBy(c => c.Order)
                    .Take(5)
                    .Select(c => c.Name);

                var vm = new ImportMovieViewModel
                {
                    TmdbId = movie.Id,
                    Titulo = movie.Title,
                    TituloOriginal = movie.OriginalTitle,
                    Sinopse = movie.Overview,
                    DataLancamento = movie.ReleaseDate,
                    Genero = movie.Genres != null ? string.Join(", ", movie.Genres.Select(g => g.Name)) : null,
                    PosterPath = movie.PosterPath,
                    Lingua = movie.OriginalLanguage,
                    Duracao = movie.Runtime,
                    NotaMedia = movie.VoteAverage,
                    ElencoPrincipal = elenco != null ? string.Join(", ", elenco) : null
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao preparar importação do filme TMDbId={Id}", tmdbId);
                return BadRequest("Erro ao carregar dados do filme.");
            }
        }

        // POST /Movies/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ImportMovieViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido ao importar filme. TmdbId={Id}", vm.TmdbId);
                return View(vm);
            }

            var existente = await _filmes.ReadAsync(vm.TmdbId);
            if (existente != null)
            {
                _logger.LogWarning("Filme já existente no banco. TmdbId={Id}, LocalId={LocalId}",
                    vm.TmdbId, existente.Id);

                return RedirectToAction(nameof(DetailsLocal), new { id = existente.Id });
            }

            var filme = new Filme
            {
                TmdbId = vm.TmdbId,
                Titulo = vm.Titulo,
                TituloOriginal = vm.TituloOriginal,
                Sinopse = vm.Sinopse,
                DataLancamento = vm.DataLancamento,
                Genero = vm.Genero,
                PosterPath = vm.PosterPath,
                Lingua = vm.Lingua,
                Duracao = vm.Duracao,
                NotaMedia = vm.NotaMedia.HasValue ? (decimal?)vm.NotaMedia.Value : null,
                ElencoPrincipal = vm.ElencoPrincipal,
                CidadeReferencia = vm.CidadeReferencia,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                DataCriacao = DateTimeOffset.UtcNow,
                DataAtualizacao = DateTimeOffset.UtcNow
            };

            try
            {
                await _filmes.CreateAsync(filme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao criar filme no banco. TmdbId={Id} Título={Titulo}", vm.TmdbId, vm.Titulo);

                ModelState.AddModelError("", "Erro ao salvar filme.");
                return View(vm);
            }

            return RedirectToAction(nameof(DetailsLocal), new { id = filme.Id });
        }

        // detalhe do filme salvo localmente
        public async Task<IActionResult> DetailsLocal(int id)
        {
            var filme = await _filmes.GetByIdAsync(id);

            if (filme == null)
            {
                _logger.LogWarning("Filme local não encontrado. Id={Id}", id);
                return NotFound();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
