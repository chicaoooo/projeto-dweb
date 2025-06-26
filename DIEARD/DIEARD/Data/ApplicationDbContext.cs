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
        // 'importa' todo o comportamento do método, 
        // aquando da sua definição na SuperClasse
        base.OnModelCreating(modelBuilder);

        // Configuração da tabela Diarios
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

            // Relacionamento Diario -> User
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento Diario -> Categoria
            entity.HasOne(d => d.Categoria)
                .WithMany(c => c.Diarios)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da tabela Categorias
        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Descricao)
                .HasMaxLength(500);
        });

        // Configuração da tabela Utilizadores
        modelBuilder.Entity<Utilizadores>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId)
                .IsRequired();
            // Ignorar propriedade Email (está no IdentityUser)
            entity.Ignore(e => e.Email);
            // Índice único para UserId
            entity.HasIndex(u => u.UserId)
                .IsUnique();
        });

        // Configuração da tabela Amizades
        modelBuilder.Entity<Amizades>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UtilizadorId)
                .IsRequired();

            entity.Property(e => e.AmigoId)
                .IsRequired();

            entity.Property(e => e.DataAmizade)
                .IsRequired();

            // Relacionamentos com NO ACTION para evitar ciclos
            entity.HasOne(a => a.Utilizador)
                .WithMany()
                .HasForeignKey(a => a.UtilizadorId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.Amigo)
                .WithMany()
                .HasForeignKey(a => a.AmigoId)
                .OnDelete(DeleteBehavior.NoAction);

            // Índice único para evitar amizades duplicadas
            entity.HasIndex(a => new { a.UtilizadorId, a.AmigoId })
                .IsUnique();
        });

        // 🆕 NOVA: Configuração da tabela PedidosAmizade
        modelBuilder.Entity<PedidosAmizade>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RemetenteId).IsRequired();
            entity.Property(e => e.DestinatarioId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.DataPedido).IsRequired();

            // Relacionamentos
            entity.HasOne(p => p.Remetente)
                .WithMany()
                .HasForeignKey(p => p.RemetenteId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(p => p.Destinatario)
                .WithMany()
                .HasForeignKey(p => p.DestinatarioId)
                .OnDelete(DeleteBehavior.NoAction);

            // Índice único para evitar pedidos duplicados
            entity.HasIndex(p => new { p.RemetenteId, p.DestinatarioId }).IsUnique();
        });

        // Criar os perfis de utilizador da nossa app
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "a", Name = "Administrador", NormalizedName = "ADMINISTRADOR" });

        // Criar um utilizador para funcionar como ADMIN
        // Função para codificar a password
        var hasher = new PasswordHasher<IdentityUser>();

        // Criação do utilizador
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

        // Associar este utilizador à role ADMIN
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = "admin", RoleId = "a" });

        // Seed de categorias padrão
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
    public DbSet<PedidosAmizade> PedidosAmizade { get; set; } // 🆕 NOVA linha
}