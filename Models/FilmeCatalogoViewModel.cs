using System.Collections.Generic;

namespace catalogo_filmes_previsao_tempo.Models;

public class FilmeCatalogoViewModel
{
    public IEnumerable<Filme> Filmes { get; set; } = new List<Filme>();

    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }

    public int TotalPaginas => 
        (int)System.Math.Ceiling((double)TotalRegistros / TamanhoPagina);

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;
}