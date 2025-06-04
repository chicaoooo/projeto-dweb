using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da categoria não pode exceder 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(255, ErrorMessage = "A descrição não pode exceder 255 caracteres")]
        public string? Descricao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamento com Diários
        public ICollection<Diario> Diarios { get; set; } = new List<Diario>();
    }
}