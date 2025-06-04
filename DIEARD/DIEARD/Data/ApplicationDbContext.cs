using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DIEARD.Models;

namespace DIEARD.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Diario> Diarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; } // Nova tabela

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Relacionamento Diario -> User
        builder.Entity<Diario>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento Diario -> Categoria
        builder.Entity<Diario>()
            .HasOne(d => d.Categoria)
            .WithMany(c => c.Diarios)
            .HasForeignKey(d => d.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento UserProfile -> User
        builder.Entity<UserProfile>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único para UserProfile
        builder.Entity<UserProfile>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        // Seed de categorias padrão
        builder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Pessoal", Descricao = "Pensamentos e experiências pessoais" },
            new Categoria { Id = 2, Nome = "Trabalho", Descricao = "Relacionado ao trabalho e carreira" },
            new Categoria { Id = 3, Nome = "Saúde", Descricao = "Bem-estar físico e mental" },
            new Categoria { Id = 4, Nome = "Viagens", Descricao = "Experiências de viagem" },
            new Categoria { Id = 5, Nome = "Família", Descricao = "Momentos em família" }
        );
    }
}