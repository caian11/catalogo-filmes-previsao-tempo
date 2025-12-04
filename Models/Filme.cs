namespace catalogo_filmes_previsao_tempo.Models;

public class Filme
{
    public int Id { get; set; }
    public int TmdbId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? TituloOriginal { get; set; }
    public string? Sinopse { get; set; }
    public DateTime? DataLancamento { get; set; }
    public string? Genero { get; set; }
    public string? PosterPath { get; set; }
    public string? Lingua { get; set; }
    public int? Duracao { get; set; }
    public decimal? NotaMedia { get; set; }
    public string? ElencoPrincipal { get; set; }
    public string? CidadeReferencia { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // troque para DateTimeOffset
    public DateTimeOffset DataCriacao { get; set; }
    public DateTimeOffset DataAtualizacao { get; set; }
}

