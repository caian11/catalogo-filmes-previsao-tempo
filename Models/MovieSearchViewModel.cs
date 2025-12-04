namespace catalogo_filmes_previsao_tempo.Models;

public class MovieSearchViewModel
{
    public string? Query { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }

    public List<MovieListItemViewModel> Results { get; set; } = new();
}