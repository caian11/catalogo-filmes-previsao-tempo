using System.Collections.Generic;
using System.Threading.Tasks;
using catalogo_filmes_previsao_tempo.Models;

namespace catalogo_filmes_previsao_tempo.Data
{
    public interface IFilmeRepository
    {
        // CREATE
        Task<Filme> CreateAsync(Filme filme);

        // READ (exemplo: ler por TMDbId – útil pro Import)
        Task<Filme?> ReadAsync(int tmdbId);

        // UPDATE
        Task UpdateAsync(Filme filme);

        // DELETE
        Task DeleteAsync(int id);

        // LIST (todos)
        Task<List<Filme>> ListAsync();

        // GET BY ID (PK local)
        Task<Filme?> GetByIdAsync(int id);
    }
}