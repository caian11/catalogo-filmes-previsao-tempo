using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using catalogo_filmes_previsao_tempo.Data;
using catalogo_filmes_previsao_tempo.Models;
using ClosedXML.Excel;

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
        public async Task<IActionResult> Index()
        {
            var lista = await _filmes.ListAsync();
            return View(lista);
        }

        // GET /Filmes/Export?formato=csv|xlsx
        [HttpGet]
        public async Task<IActionResult> Export(string formato = "csv")
        {
            var lista = await _filmes.ListAsync();

            if (string.Equals(formato, "xlsx", StringComparison.OrdinalIgnoreCase))
                return ExportarExcel(lista);

            return ExportarCsv(lista);
        }

        private FileResult ExportarCsv(IEnumerable<Filme> filmes)
        {
            var sb = new StringBuilder();

            // cabeçalho
            sb.AppendLine("Id;Titulo;Genero;DataLancamento;CidadeReferencia");

            foreach (var f in filmes)
            {
                var titulo = (f.Titulo ?? "").Replace("\"", "\"\"");
                var genero = (f.Genero ?? "").Replace("\"", "\"\"");
                var cidade = (f.CidadeReferencia ?? "").Replace("\"", "\"\"");
                var dataLanc = f.DataLancamento.HasValue
                    ? f.DataLancamento.Value.ToString("yyyy-MM-dd")
                    : "";

                sb.AppendLine(
                    $"{f.Id};\"{titulo}\";\"{genero}\";\"{dataLanc}\";\"{cidade}\""
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"filmes_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        private FileResult ExportarExcel(IEnumerable<Filme> filmes)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Filmes");

            // cabeçalho
            ws.Cell(1, 1).Value = "Id";
            ws.Cell(1, 2).Value = "Título";
            ws.Cell(1, 3).Value = "Gênero";
            ws.Cell(1, 4).Value = "Data lançamento";
            ws.Cell(1, 5).Value = "Cidade referência";

            var linha = 2;
            foreach (var f in filmes)
            {
                ws.Cell(linha, 1).Value = f.Id;
                ws.Cell(linha, 2).Value = f.Titulo ?? "";
                ws.Cell(linha, 3).Value = f.Genero ?? "";
                ws.Cell(linha, 4).Value = f.DataLancamento;
                ws.Cell(linha, 5).Value = f.CidadeReferencia ?? "";
                linha++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var conteudo = stream.ToArray();
            var fileName = $"filmes_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

            return File(
                conteudo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // GET /Filmes/Details/5 
        public async Task<IActionResult> Details(int id)
        {
            var filme = await _filmes.GetByIdAsync(id);
            if (filme == null) return NotFound();

            return View(filme);
        }

        // POST /Filmes/BuscarPorId  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuscarPorId(int id)
        {
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

        // POST /Filmes/Delete/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _filmes.DeleteAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
