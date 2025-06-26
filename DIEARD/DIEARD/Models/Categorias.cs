using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    public class Categorias
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string? Descricao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public ICollection<Diarios> Diarios { get; set; } = new List<Diarios>();
    }

}