using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIEARD.Models
{
    public class Diario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Conteudo { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamento com User
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Relacionamento com Categoria
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}