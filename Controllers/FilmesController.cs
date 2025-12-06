using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using catalogo_filmes_previsao_tempo.Data;
using catalogo_filmes_previsao_tempo.Models;

namespace catalogo_filmes_previsao_tempo.Controllers
{
    public class FilmesController : Controller
    {
        private readonly IFilmeRepository _filmes;

        public FilmesController(IFilmeRepository filmes)
        {
            _filmes = filmes;
        }

        // GET /Filmes
        // Usa exatamente a view que você mostrou (Index.cshtml)
        public async Task<IActionResult> Index()
        {
            var lista = await _filmes.ListAsync();
            return View(lista);
        }

        // GET /Filmes/Details/5 (buscar apenas um por id)
        public async Task<IActionResult> Details(int id)
        {
            var filme = await _filmes.GetByIdAsync(id);
            if (filme == null) return NotFound();

            return View(filme);
        }

        // POST /Filmes/BuscarPorId  (form na própria Index)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarPorId(int id)
        {
            // redireciona pra Details, que já faz o GetById
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /Filmes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var filme = await _filmes.GetByIdAsync(id);
            if (filme == null) return NotFound();

            return View(filme);
        }

        // POST /Filmes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Filme filme)
        {
            if (id != filme.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(filme);

            filme.DataAtualizacao = DateTimeOffset.UtcNow;
            await _filmes.UpdateAsync(filme);

            return RedirectToAction("Index", "Home");
        }

        // POST /Filmes/Delete/5 (delete direto a partir da Index)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _filmes.DeleteAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
