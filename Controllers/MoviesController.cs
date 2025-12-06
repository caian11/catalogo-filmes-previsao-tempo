using Microsoft.AspNetCore.Mvc;
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

        public MoviesController(ITmdbApiService tmdb, IFilmeRepository filmes)
        {
            _tmdb = tmdb;
            _filmes = filmes;
        }

        // GET /Movies/Search?query=...&page=1
        public async Task<IActionResult> Search(string? query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new MovieSearchViewModel { Query = "", Page = 1, TotalPages = 0 });

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

        // GET /Movies/Details?tmdbId=123
        public async Task<IActionResult> Details(int tmdbId)
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

        // GET /Movies/Import?tmdbId=123 
        [HttpGet]
        public async Task<IActionResult> Import(int tmdbId)
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

        // POST /Movies/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ImportMovieViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            
            var existente = await _filmes.ReadAsync(vm.TmdbId);
            if (existente != null)
                return RedirectToAction(nameof(DetailsLocal), new { id = existente.Id });

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

            await _filmes.CreateAsync(filme);

            return RedirectToAction(nameof(DetailsLocal), new { id = filme.Id });
        }

        // detalhe do filme salvo localmente
        public async Task<IActionResult> DetailsLocal(int id)
        {
            var filme = await _filmes.GetByIdAsync(id);
            if (filme == null) return NotFound();

            return RedirectToAction("Index", "Home");
        }
    }
}
