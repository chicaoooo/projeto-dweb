using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DIEARD.Models
{
    public class Diario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo é obrigatório.")]
        [Display(Name = "Conteúdo")]
        public string Conteudo { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; }

        [Required]
        public string UserId { get; set; }

        [BindNever]
        public IdentityUser User { get; set; }
    }
}