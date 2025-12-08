namespace catalogo_filmes_previsao_tempo.Models;

public class Daily
{
    public List<string> time { get; set; }
    public List<double> temperature_2m_min { get; set; }
    public List<double> temperature_2m_max { get; set; }
}