using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DIEARD.Data;
using DIEARD.Models;

namespace DIEARD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Construtor que injeta as dependências do controlador.
        public HomeController(ApplicationDbContext context,
                             UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Exibe a página principal com um resumo das informações do utilizador.
        public async Task<IActionResult> Index()
        {
            ViewBag.UtilizadorNome = "Visitante";
            ViewBag.NumeroDiarios = 0;
            ViewBag.NumeroAmigos = 0;
            ViewBag.PedidosPendentes = 0;
            ViewBag.IsAdmin = false;

            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    await LogoutSilencioso();
                    return View();
                }

                var utilizador = await GetUtilizadorSeguro(userId);
                if (utilizador == null)
                {
                    await LogoutSilencioso();
                    TempData["ErrorMessage"] = "Sessão expirou. Faça login novamente.";
                    return View();
                }

                ViewBag.UtilizadorNome = utilizador.UserName ?? "Utilizador";
                await CarregarDadosSeguro(userId, utilizador);
            }
            catch (Exception)
            {
                await LogoutSilencioso();
                TempData["ErrorMessage"] = "Erro na sessão. Tente fazer login novamente.";
            }

            return View();
        }

        // Obtém de forma segura o utilizador da base de dados pelo seu ID.
        private async Task<IdentityUser> GetUtilizadorSeguro(string userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Carrega as estatísticas (diários, amigos, etc.) para a ViewBag.
        private async Task CarregarDadosSeguro(string userId, IdentityUser utilizador)
        {
            try
            {
                ViewBag.NumeroDiarios = await _context.Diarios
                    .Where(d => d.UserId == userId)
                    .CountAsync();
            }
            catch (Exception)
            {
                ViewBag.NumeroDiarios = 0;
            }

            try
            {
                ViewBag.NumeroAmigos = await _context.Amizades
                    .Where(a => a.UtilizadorId == userId || a.AmigoId == userId)
                    .CountAsync();
            }
            catch (Exception)
            {
                ViewBag.NumeroAmigos = 0;
            }

            try
            {
                ViewBag.PedidosPendentes = await _context.PedidosAmizade
                    .Where(p => p.DestinatarioId == userId && p.Status == StatusPedido.Pendente)
                    .CountAsync();
            }
            catch (Exception)
            {
                ViewBag.PedidosPendentes = 0;
            }

            try
            {
                ViewBag.IsAdmin = await _userManager.IsInRoleAsync(utilizador, "Admin");
            }
            catch (Exception)
            {
                ViewBag.IsAdmin = false;
            }
        }

        // GET: Realiza a pesquisa de utilizadores e exibe os resultados.
        [HttpGet("/search")]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<IdentityUser>());
            }

            var users = _userManager.Users
                                   .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
                                   .ToList();

            return View("Search", users);
        }

        // Realiza o logout do utilizador sem lançar exceções.
        private async Task LogoutSilencioso()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception)
            {
                // Continuar mesmo se o logout falhar
            }
        }

        // GET: Exibe a página de política de privacidade.
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Força o logout do utilizador para limpar uma sessão corrompida.
        public async Task<IActionResult> Emergency()
        {
            try
            {
                await _signInManager.SignOutAsync();
                TempData["SuccessMessage"] = "Sessão limpa com sucesso.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Erro ao limpar sessão.";
            }

            return RedirectToAction("Index");
        }
    }
}