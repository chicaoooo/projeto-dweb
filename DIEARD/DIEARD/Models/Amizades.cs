using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    public class Amizades
    {
        public int Id { get; set; }

        [Required]
        public string UtilizadorId { get; set; } // Um dos amigos

        [Required]
        public string AmigoId { get; set; } // O outro amigo

        public DateTime DataAmizade { get; set; }

        // Propriedades de navegação
        public IdentityUser Utilizador { get; set; }
        public IdentityUser Amigo { get; set; }
    }
}

