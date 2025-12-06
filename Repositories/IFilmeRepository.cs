using System.Collections.Generic;
using System.Threading.Tasks;
using catalogo_filmes_previsao_tempo.Models;

namespace catalogo_filmes_previsao_tempo.Data
{
    public interface IFilmeRepository
    {
        // CREATE
        Task<Filme> CreateAsync(Filme filme);

        // READ 
        Task<Filme?> ReadAsync(int tmdbId);

        // UPDATE
        Task UpdateAsync(Filme filme);

        // DELETE
        Task DeleteAsync(int id);

        // LIST 
        Task<List<Filme>> ListAsync();

        // GET BY ID 
        Task<Filme?> GetByIdAsync(int id);
    }
}