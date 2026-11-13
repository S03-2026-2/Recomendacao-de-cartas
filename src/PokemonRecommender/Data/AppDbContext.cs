using Microsoft.EntityFrameworkCore;
using PokemonRecommender.Models;

namespace PokemonRecommender.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RateLimit> RateLimits => Set<RateLimit>();
    public DbSet<Carta> Cartas => Set<Carta>();
    public DbSet<Regra> Regras => Set<Regra>();
    public DbSet<EfetividadeTipo> Efetividades => Set<EfetividadeTipo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasPostgresExtension("vector");

        b.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.HasDiscriminator<string>("TipoUsuario")
             .HasValue<Jogador>("jogador")
             .HasValue<Admin>("admin");
        });

        b.Entity<Jogador>(e =>
        {
            e.Property(x => x.Nickname).HasMaxLength(50);
            e.HasIndex(x => x.Nickname).IsUnique();
            e.HasOne(x => x.RateLimit)
             .WithOne()
             .HasForeignKey<RateLimit>(r => r.UsuarioId);
        });

        b.Entity<RateLimit>(e =>
        {
            e.HasKey(x => x.UsuarioId);
            e.Property(x => x.UsuarioId).ValueGeneratedNever();
            e.OwnsOne(x => x.Policy);
        });

        b.Entity<Carta>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
        });

        b.Entity<Regra>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Texto).IsRequired();
            e.Property(x => x.Fonte).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Fonte);
            e.Property(x => x.Embedding).HasColumnType("vector(1536)");
            e.HasIndex(x => x.Embedding)
             .HasMethod("hnsw")
             .HasOperators("vector_cosine_ops");
        });

        b.Entity<EfetividadeTipo>(e =>
        {
            e.HasKey(x => new { x.Atacante, x.Defensor });
            e.Property(x => x.Atacante).HasMaxLength(50);
            e.Property(x => x.Defensor).HasMaxLength(50);
        });
    }
}