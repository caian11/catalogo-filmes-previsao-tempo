namespace catalogo_filmes_previsao_tempo.Models;

public class MovieDetailsViewModel
{
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? TituloOriginal { get; set; }
    public string? Sinopse { get; set; }
    public DateTime? DataLancamento { get; set; }
    public string? Genero { get; set; }
    public string? PosterUrl { get; set; }
    public string? Lingua { get; set; }
    public int? Duracao { get; set; }
    public double? NotaMedia { get; set; }
    public string? ElencoPrincipal { get; set; }
}
