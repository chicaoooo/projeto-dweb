using System.ComponentModel.DataAnnotations;
namespace DIEARD.Models.ApiModels;

public class AmigoDto
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataAmizade { get; set; }
}

public class PedidoAmizadeDto
{
    public int Id { get; set; }
    public string RemetenteId { get; set; }
    public string RemetenteNome { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataPedido { get; set; }
}

public class EnviarPedidoAmizadeDto
{
    [Required]
    public string DestinatarioId { get; set; }

    public string Mensagem { get; set; }
}