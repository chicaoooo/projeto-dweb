using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIEARD.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        
        public IdentityUser User { get; set; }


        // Foto do perfil armazenada como Base64 string (VARCHAR na base de dados)
        [Column(TypeName = "varchar(max)")]
        public string? FotoPerfil { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
    }
}