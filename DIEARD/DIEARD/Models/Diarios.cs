using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIEARD.Models
{
    public class Diarios
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Conteudo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categorias? Categoria { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        
    }
}