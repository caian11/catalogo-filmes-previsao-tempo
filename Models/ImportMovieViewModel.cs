using System.ComponentModel.DataAnnotations;
namespace catalogo_filmes_previsao_tempo.Models;

public class ImportMovieViewModel
{
    public int TmdbId { get; set; }

    [Required]
    public string Titulo { get; set; } = null!;

    public string? TituloOriginal { get; set; }
    public string? Sinopse { get; set; }
    public DateTime? DataLancamento { get; set; }
    public string? Genero { get; set; }
    public string? PosterPath { get; set; }
    public string? Lingua { get; set; }
    public int? Duracao { get; set; }
    public double? NotaMedia { get; set; }
    public string? ElencoPrincipal { get; set; }

    // preenchidos manualmente ou com algum default
    public string? CidadeReferencia { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
