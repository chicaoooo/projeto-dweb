using DIEARD.Data;
using DIEARD.Models;
using DIEARD.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace DIEARD.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AmizadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AmizadesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmigoDto>>> GetAmigos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var amigos = await _context.Amizades
                .Where(a => a.UtilizadorId == userId || a.AmigoId == userId)
                .Select(a => new AmigoDto
                {
                    Id = a.UtilizadorId == userId ? a.AmigoId : a.UtilizadorId,
                    Nome = a.UtilizadorId == userId ? a.Amigo.UserName : a.Utilizador.UserName,
                    DataAmizade = a.DataAmizade
                })
                .ToListAsync();

            return Ok(amigos);
        }

        [HttpGet("pedidos/recebidos")]
        public async Task<ActionResult<IEnumerable<PedidoAmizadeDto>>> GetPedidosRecebidos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedidos = await _context.PedidosAmizade
                .Where(p => p.DestinatarioId == userId && p.Status == StatusPedido.Pendente)
                .Select(p => new PedidoAmizadeDto
                {
                    Id = p.Id,
                    RemetenteId = p.RemetenteId,
                    RemetenteNome = p.Remetente.UserName,
                    Mensagem = p.Mensagem,
                    DataPedido = p.DataPedido
                })
                .ToListAsync();

            return Ok(pedidos);
        }

        [HttpPost("pedidos/enviar")]
        public async Task<ActionResult> EnviarPedidoAmizade([FromBody] EnviarPedidoAmizadeDto pedidoDto)
        {
            var remetenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (remetenteId == pedidoDto.DestinatarioId)
                return BadRequest("Não pode enviar pedido para si mesmo");

            var jaSaoAmigos = await _context.Amizades
                .AnyAsync(a =>
                    (a.UtilizadorId == remetenteId && a.AmigoId == pedidoDto.DestinatarioId) ||
                    (a.UtilizadorId == pedidoDto.DestinatarioId && a.AmigoId == remetenteId));

            if (jaSaoAmigos)
                return BadRequest("Já são amigos");

            var pedidoPendente = await _context.PedidosAmizade
                .AnyAsync(p =>
                    (p.RemetenteId == remetenteId && p.DestinatarioId == pedidoDto.DestinatarioId) ||
                    (p.RemetenteId == pedidoDto.DestinatarioId && p.DestinatarioId == remetenteId) &&
                    p.Status == StatusPedido.Pendente);

            if (pedidoPendente)
                return BadRequest("Já existe um pedido pendente entre vocês");

            var pedido = new PedidosAmizade
            {
                RemetenteId = remetenteId,
                DestinatarioId = pedidoDto.DestinatarioId,
                Mensagem = pedidoDto.Mensagem,
                DataPedido = DateTime.Now,
                Status = StatusPedido.Pendente
            };

            _context.PedidosAmizade.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Pedido enviado com sucesso" });
        }

        [HttpPost("pedidos/{id}/aceitar")]
        public async Task<ActionResult> AceitarPedidoAmizade(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedido = await _context.PedidosAmizade
                .FirstOrDefaultAsync(p => p.Id == id && p.DestinatarioId == userId && p.Status == StatusPedido.Pendente);

            if (pedido == null)
                return NotFound("Pedido não encontrado");

            // Criar amizade
            var amizade = new Amizades
            {
                UtilizadorId = pedido.RemetenteId,
                AmigoId = pedido.DestinatarioId,
                DataAmizade = DateTime.Now
            };

            _context.Amizades.Add(amizade);
            _context.PedidosAmizade.Remove(pedido); // Remover o pedido após aceitar

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Pedido aceito com sucesso" });
        }

        [HttpDelete("{amigoId}")]
        public async Task<ActionResult> RemoverAmizade(string amigoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var amizade = await _context.Amizades
                .FirstOrDefaultAsync(a =>
                    (a.UtilizadorId == userId && a.AmigoId == amigoId) ||
                    (a.UtilizadorId == amigoId && a.AmigoId == userId));

            if (amizade == null)
                return NotFound("Amizade não encontrada");

            _context.Amizades.Remove(amizade);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Amizade removida com sucesso" });
        }
    }
}

