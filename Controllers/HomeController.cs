using System.Diagnostics;
using catalogo_filmes_previsao_tempo.Data;
using Microsoft.AspNetCore.Mvc;
using catalogo_filmes_previsao_tempo.Models;
using Microsoft.EntityFrameworkCore;

namespace catalogo_filmes_previsao_tempo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFilmeRepository _filmes;

    public HomeController(
        ILogger<HomeController> logger,
        IFilmeRepository filmes   // injeta o repositório aqui
    )
    {
        _logger = logger;
        _filmes = filmes;
    }

    // GET /
    public async Task<IActionResult> Index(int pagina = 1, int tamanhoPagina = 12)
    {
        if (pagina < 1) pagina = 1;

        var totalRegistros = await _filmes.CountAsync();
        var filmes = await _filmes.ListPagedAsync(pagina, tamanhoPagina);

        var vm = new FilmeCatalogoViewModel
        {
            Filmes = filmes,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = totalRegistros
        };

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}