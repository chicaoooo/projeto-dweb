using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace DIEARD
{
    public class Diario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Conteudo { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}