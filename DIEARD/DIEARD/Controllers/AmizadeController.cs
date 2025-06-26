using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DIEARD.Data;
using DIEARD.Models;
using System.Security.Claims;

namespace DIEARD.Controllers
{
    [Authorize]
    public class AmizadeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Construtor que injeta as dependências do controlador.
        public AmizadeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Verifica se dois utilizadores têm uma amizade aceite.
        private async Task<bool> SaoAmigos(string userId1, string userId2)
        {
            if (userId1 == userId2) return true;

            return await _context.Amizades
                .AnyAsync(a =>
                    (a.UtilizadorId == userId1 && a.AmigoId == userId2) ||
                    (a.UtilizadorId == userId2 && a.AmigoId == userId1));
        }

        // Verifica se já existe um pedido de amizade pendente entre dois utilizadores.
        private async Task<bool> ExistePedidoPendente(string remetenteId, string destinatarioId)
        {
            return await _context.PedidosAmizade
                .AnyAsync(p =>
                    ((p.RemetenteId == remetenteId && p.DestinatarioId == destinatarioId) ||
                     (p.RemetenteId == destinatarioId && p.DestinatarioId == remetenteId)) &&
                    p.Status == StatusPedido.Pendente);
        }

        // GET: Exibe a lista de amigos do utilizador atual.
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var minhasAmizades = await _context.Amizades
                .Where(a => a.UtilizadorId == userId || a.AmigoId == userId)
                .Include(a => a.Utilizador)
                .Include(a => a.Amigo)
                .ToListAsync();

            var amigos = minhasAmizades.Select(a =>
                a.UtilizadorId == userId ? a.Amigo : a.Utilizador
            ).ToList();

            ViewBag.Amigos = amigos;
            return View();
        }

        // GET: Exibe a lista de pedidos de amizade recebidos pelo utilizador.
        public async Task<IActionResult> PedidosRecebidos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await LimparPedidosProcessados();

            var pedidosRecebidos = await _context.PedidosAmizade
                .Where(p => p.DestinatarioId == userId && p.Status == StatusPedido.Pendente)
                .Include(p => p.Remetente)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return View(pedidosRecebidos);
        }

        // GET: Exibe a lista de pedidos de amizade enviados pelo utilizador.
        public async Task<IActionResult> PedidosEnviados()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await LimparPedidosProcessados();

            var pedidosEnviados = await _context.PedidosAmizade
                .Where(p => p.RemetenteId == userId)
                .Include(p => p.Destinatario)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return View(pedidosEnviados);
        }

        // POST: Envia um novo pedido de amizade para um utilizador.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarPedido(string destinatarioId, string mensagem = "")
        {
            var remetenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(destinatarioId) || remetenteId == destinatarioId)
            {
                TempData["ErrorMessage"] = "Utilizador inválido.";
                return RedirectToAction("Index", "Amizade");
            }

            if (await SaoAmigos(remetenteId, destinatarioId))
            {
                TempData["ErrorMessage"] = "Vocês já são amigos.";
                return RedirectToAction("Index", "Amizade");
            }

            if (await ExistePedidoPendente(remetenteId, destinatarioId))
            {
                TempData["ErrorMessage"] = "Já existe um pedido de amizade pendente.";
                return RedirectToAction("Index", "Amizade");
            }

            try
            {
                var novoPedido = new PedidosAmizade
                {
                    RemetenteId = remetenteId,
                    DestinatarioId = destinatarioId,
                    Mensagem = mensagem,
                    DataPedido = DateTime.Now,
                    Status = StatusPedido.Pendente
                };

                _context.PedidosAmizade.Add(novoPedido);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pedido de amizade enviado com sucesso!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao enviar pedido. Tente novamente.";
            }

            return RedirectToAction("Index", "Amizade");
        }

        // POST: Aceita um pedido de amizade e cria a relação de amizade.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarPedido(int pedidoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedido = await _context.PedidosAmizade
                .FirstOrDefaultAsync(p => p.Id == pedidoId &&
                                        p.DestinatarioId == userId &&
                                        p.Status == StatusPedido.Pendente);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido não encontrado.";
                return RedirectToAction("PedidosRecebidos");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var novaAmizade = new Amizades
                {
                    UtilizadorId = pedido.RemetenteId,
                    AmigoId = pedido.DestinatarioId,
                    DataAmizade = DateTime.Now
                };
                _context.Amizades.Add(novaAmizade);
                _context.PedidosAmizade.Remove(pedido);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Pedido aceite! Agora são amigos.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Erro ao aceitar pedido.";
            }

            return RedirectToAction("PedidosRecebidos");
        }

        // POST: Recusa e remove um pedido de amizade recebido.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecusarPedido(int pedidoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedido = await _context.PedidosAmizade
                .FirstOrDefaultAsync(p => p.Id == pedidoId &&
                                        p.DestinatarioId == userId &&
                                        p.Status == StatusPedido.Pendente);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido não encontrado.";
                return RedirectToAction("PedidosRecebidos");
            }

            _context.PedidosAmizade.Remove(pedido);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pedido recusado.";
            return RedirectToAction("PedidosRecebidos");
        }

        // POST: Cancela e remove um pedido de amizade enviado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarPedido(int pedidoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pedido = await _context.PedidosAmizade
                .FirstOrDefaultAsync(p => p.Id == pedidoId &&
                                        p.RemetenteId == userId &&
                                        p.Status == StatusPedido.Pendente);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "Pedido não encontrado.";
                return RedirectToAction("PedidosEnviados");
            }

            _context.PedidosAmizade.Remove(pedido);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pedido cancelado.";
            return RedirectToAction("PedidosEnviados");
        }

        // POST: Desfaz uma amizade existente entre dois utilizadores.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverAmigo(string amigoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var amizade = await _context.Amizades
                .FirstOrDefaultAsync(a =>
                    (a.UtilizadorId == userId && a.AmigoId == amigoId) ||
                    (a.UtilizadorId == amigoId && a.AmigoId == userId));

            if (amizade == null)
            {
                TempData["ErrorMessage"] = "Amizade não encontrada.";
                return RedirectToAction("Index");
            }

            _context.Amizades.Remove(amizade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Amigo removido com sucesso.";
            return RedirectToAction("Index");
        }

        // POST: Redireciona chamadas de um método antigo para a funcionalidade de pesquisa.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdicionarAmigo(string amigoId)
        {
            TempData["ErrorMessage"] = "Sistema atualizado! Agora você precisa enviar um pedido de amizade que será aprovado pelo destinatário.";
            return RedirectToAction("Search", new { query = "" });
        }

        // Remove da base de dados os pedidos que já foram processados.
        private async Task LimparPedidosProcessados()
        {
            try
            {
                var pedidosAceites = await _context.PedidosAmizade
                    .Where(p => p.Status == StatusPedido.Aceite)
                    .ToListAsync();

                foreach (var pedido in pedidosAceites)
                {
                    var amizadeExiste = await _context.Amizades
                        .AnyAsync(a =>
                            (a.UtilizadorId == pedido.RemetenteId && a.AmigoId == pedido.DestinatarioId) ||
                            (a.UtilizadorId == pedido.DestinatarioId && a.AmigoId == pedido.RemetenteId));

                    if (amizadeExiste)
                    {
                        _context.PedidosAmizade.Remove(pedido);
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar erros na limpeza silenciosamente
            }
        }

        // GET: Redireciona para a página de diários de um amigo específico.
        public async Task<IActionResult> DiariosDo(string amigoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isAdmin = User.IsInRole("Administrador");
            if (!isAdmin && !await SaoAmigos(userId, amigoId))
            {
                TempData["ErrorMessage"] = "Você só pode ver diários de amigos.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("UserDiaries", "Diario", new { userId = amigoId });
        }
    }
}