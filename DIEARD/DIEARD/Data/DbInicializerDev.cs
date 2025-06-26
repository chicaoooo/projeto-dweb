
using DIEARD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DIEARD.Data
{
    public class DbInicializerDev
    {
        internal static async Task Initialize(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            await dbContext.Database.EnsureCreatedAsync();

            // 1. Criar Perfil de Administrador
            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR"
                });
            }

            // 2. Criar Utilizador Admin
            var adminUser = await userManager.FindByEmailAsync("admin@mail.pt");
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    Id = "admin",
                    UserName = "admin@mail.pt",
                    NormalizedUserName = "ADMIN@MAIL.PT",
                    Email = "admin@mail.pt",
                    NormalizedEmail = "ADMIN@MAIL.PT",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(adminUser, "Aa0_aa");
                if (!result.Succeeded)
                {
                    throw new Exception("Falha ao criar usuário admin");
                }
            }

            // 3. Associar Admin à Role
            if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }

            // 4. Criar os 3 utilizadores normais
            string[] userEmails = {
                "joao.mendes@mail.pt",
                "maria.sousa@mail.pt",
                "ana.silva@mail.pt"
            };

            foreach (var email in userEmails)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = email,
                        NormalizedUserName = email.ToUpper(),
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("N").ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };

                    var result = await userManager.CreateAsync(user, "Aa0_aa");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Falha ao criar usuário {email}");
                    }
                }
            }

            // 5. Criar registos na tabela Utilizadores
            if (!await dbContext.Utilizadores.AnyAsync())
            {
                var utilizadores = new[]
                {
                    new Utilizadores
                    {
                        Nome = "João Mendes",
                        Morada = "Rua das Flores, 45",
                        CodPostal = "2300-000 TOMAR",
                    
                        Telemovel = "919876543",
                        
                    },
                    new Utilizadores
                    {
                        Nome = "Maria Sousa",
                        
                        
                        Telemovel = "919876543"
                    },
                    new Utilizadores
                    {
                        Nome = "Ana Paula Silva",
                        
                        Telemovel = "935678921",
                        
                    }
                };

                await dbContext.Utilizadores.AddRangeAsync(utilizadores);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}