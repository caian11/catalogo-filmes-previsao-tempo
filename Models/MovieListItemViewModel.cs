namespace catalogo_filmes_previsao_tempo.Models;

public class MovieListItemViewModel
{
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Sinopse { get; set; }
    public DateTime? DataLancamento { get; set; }
    public string? PosterUrl { get; set; }
}