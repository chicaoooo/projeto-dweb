using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    public class PedidosAmizade
    {
        public int Id { get; set; }

        [Required]
        public string RemetenteId { get; set; } // Quem enviou o pedido

        [Required]
        public string DestinatarioId { get; set; } // Quem vai receber o pedido

        public DateTime DataPedido { get; set; } = DateTime.Now;

        public DateTime? DataResposta { get; set; }

        [Required]
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;

        public string? Mensagem { get; set; }

        // Propriedades de navegação
        public IdentityUser Remetente { get; set; }
        public IdentityUser Destinatario { get; set; }
    }

    public enum StatusPedido
    {
        Pendente = 0,
        Aceite = 1,
        Recusado = 2,
        Cancelado = 3
    }
}