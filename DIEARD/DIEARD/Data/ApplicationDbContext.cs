using DIEARD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DIEARD.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Diarios>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Conteudo)
                .IsRequired();
            entity.Property(e => e.UserId)
                .IsRequired();
            entity.Property(e => e.CategoriaId)
                .IsRequired();
            entity.Property(e => e.DataCriacao)
                .IsRequired();

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Categoria)
                .WithMany(c => c.Diarios)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Descricao)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Utilizadores>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId)
                .IsRequired();
            entity.Ignore(e => e.Email);
            entity.HasIndex(u => u.UserId)
                .IsUnique();
        });

        modelBuilder.Entity<Amizades>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UtilizadorId)
                .IsRequired();
            entity.Property(e => e.AmigoId)
                .IsRequired();
            entity.Property(e => e.DataAmizade)
                .IsRequired();

            entity.HasOne(a => a.Utilizador)
                .WithMany()
                .HasForeignKey(a => a.UtilizadorId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.Amigo)
                .WithMany()
                .HasForeignKey(a => a.AmigoId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(a => new { a.UtilizadorId, a.AmigoId })
                .IsUnique();
        });

        modelBuilder.Entity<PedidosAmizade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RemetenteId).IsRequired();
            entity.Property(e => e.DestinatarioId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataPedido).IsRequired();

            entity.HasOne(p => p.Remetente)
                .WithMany()
                .HasForeignKey(p => p.RemetenteId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(p => p.Destinatario)
                .WithMany()
                .HasForeignKey(p => p.DestinatarioId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(p => new { p.RemetenteId, p.DestinatarioId }).IsUnique();
        });

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "a", Name = "Administrador", NormalizedName = "ADMINISTRADOR" });

        var hasher = new PasswordHasher<IdentityUser>();

        modelBuilder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = "admin",
                UserName = "admin@mail.pt",
                NormalizedUserName = "ADMIN@MAIL.PT",
                Email = "admin@mail.pt",
                NormalizedEmail = "ADMIN@MAIL.PT",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PasswordHash = hasher.HashPassword(null, "Aa0_aa")
            }
        );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });

        modelBuilder.Entity<Categorias>().HasData(
            new Categorias { Id = 1, Nome = "Pessoal", Descricao = "Pensamentos e experiências pessoais", DataCriacao = DateTime.Now },
            new Categorias { Id = 2, Nome = "Trabalho", Descricao = "Relacionado ao trabalho e carreira", DataCriacao = DateTime.Now },
            new Categorias { Id = 3, Nome = "Saúde", Descricao = "Bem-estar físico e mental", DataCriacao = DateTime.Now },
            new Categorias { Id = 4, Nome = "Viagens", Descricao = "Experiências de viagem", DataCriacao = DateTime.Now },
            new Categorias { Id = 5, Nome = "Família", Descricao = "Momentos em família", DataCriacao = DateTime.Now }
        );
    }

    public DbSet<Diarios> Diarios { get; set; }
    public DbSet<Categorias> Categorias { get; set; }
    public DbSet<Utilizadores> Utilizadores { get; set; }
    public DbSet<Amizades> Amizades { get; set; }
    public DbSet<PedidosAmizade> PedidosAmizade { get; set; }
}