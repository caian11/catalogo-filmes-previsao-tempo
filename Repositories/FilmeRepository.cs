using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using catalogo_filmes_previsao_tempo.Models;
using Microsoft.EntityFrameworkCore;

namespace catalogo_filmes_previsao_tempo.Data
{
    public class FilmeRepository : IFilmeRepository
    {
        private readonly AppDbContext _context;

        public FilmeRepository(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Filme> CreateAsync(Filme filme)
        {
            _context.Filmes.Add(filme);
            await _context.SaveChangesAsync();
            return filme;
        }

        // READ 
        public async Task<Filme?> ReadAsync(int tmdbId)
        {
            return await _context.Filmes
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
        }

        // UPDATE
        public async Task UpdateAsync(Filme filme)
        {
            _context.Filmes.Update(filme);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null)
                return;

            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
        }

        // LIST
        public async Task<List<Filme>> ListAsync()
        {
            return await _context.Filmes
                .AsNoTracking()
                .OrderBy(f => f.Titulo)
                .ToListAsync();
        }

        // GET BY ID 
        public async Task<Filme?> GetByIdAsync(int id)
        {
            return await _context.Filmes
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        
        public async Task<int> CountAsync()
        {
            return await _context.Filmes.CountAsync();
        }

        public async Task<List<Filme>> ListPagedAsync(int pagina, int tamanhoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 12;

            return await _context.Filmes
                .AsNoTracking()
                .OrderBy(f => f.Titulo)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();
        }
    }
}