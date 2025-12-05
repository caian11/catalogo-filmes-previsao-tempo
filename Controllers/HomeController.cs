using System.Diagnostics;
using catalogo_filmes_previsao_tempo.Data;
using Microsoft.AspNetCore.Mvc;
using catalogo_filmes_previsao_tempo.Models;
using Microsoft.EntityFrameworkCore;

namespace catalogo_filmes_previsao_tempo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // últimos 12 filmes cadastrados
        var filmes = await _context.Filmes
            .OrderByDescending(f => f.DataCriacao)
            .Take(12)
            .ToListAsync();

        return View(filmes);
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