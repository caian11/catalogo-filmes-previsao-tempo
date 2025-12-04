using catalogo_filmes_previsao_tempo.Models;
using Microsoft.EntityFrameworkCore;

namespace catalogo_filmes_previsao_tempo.Data;
public class AppDbContext : DbContext
{
    public DbSet<Filme> Filmes { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<Filme>();

        e.ToTable("filmes");

        e.Property(f => f.Id).HasColumnName("id");
        e.Property(f => f.TmdbId).HasColumnName("tmdb_id");
        e.Property(f => f.Titulo).HasColumnName("titulo");
        e.Property(f => f.TituloOriginal).HasColumnName("titulo_original");
        e.Property(f => f.Sinopse).HasColumnName("sinopse");
        e.Property(f => f.DataLancamento)
            .HasColumnName("data_lancamento")
            .HasColumnType("date");

        e.Property(f => f.Genero).HasColumnName("genero");
        e.Property(f => f.PosterPath).HasColumnName("poster_path");
        e.Property(f => f.Lingua).HasColumnName("lingua");
        e.Property(f => f.Duracao).HasColumnName("duracao");
        e.Property(f => f.NotaMedia).HasColumnName("nota_media");
        e.Property(f => f.ElencoPrincipal).HasColumnName("elenco_principal");
        e.Property(f => f.CidadeReferencia).HasColumnName("cidade_referencia");
        e.Property(f => f.Latitude).HasColumnName("latitude");
        e.Property(f => f.Longitude).HasColumnName("longitude");

        e.Property(f => f.DataCriacao)
            .HasColumnName("data_criacao")
            .HasColumnType("timestamp with time zone");

        e.Property(f => f.DataAtualizacao)
            .HasColumnName("data_atualizacao")
            .HasColumnType("timestamp with time zone");

        e.HasIndex(f => f.TmdbId).IsUnique();
    }
}