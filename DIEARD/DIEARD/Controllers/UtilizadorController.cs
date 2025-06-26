using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DIEARD.Models;
using DIEARD.Data;
using Microsoft.AspNetCore.Authorization;

namespace DIEARD.Controllers
{
    [Authorize]
    public class UtilizadorController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        // Construtor que injeta as dependências do controlador.
        public UtilizadorController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Utilizador/Edit - Exibe o formulário de edição do perfil do utilizador.
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            var model = new Utilizadores();

            if (utilizador != null)
            {
                model.Nome = utilizador.Nome;
                model.Telemovel = utilizador.Telemovel;
                model.Morada = utilizador.Morada;
                model.CodPostal = utilizador.CodPostal;
            }
            else
            {
                model.Nome = user.UserName ?? "";
                model.Telemovel = "";
            }

            model.Email = user.Email ?? "";
            model.UserId = user.Id;
            return View(model);
        }

        // POST: /Utilizador/Edit - Processa a submissão do formulário de edição de perfil.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Utilizadores model)
        {
            ModelState.Remove("Email");

            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    model.Email = currentUser.Email ?? "";
                    model.UserId = currentUser.Id;
                }
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var utilizador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.UserId == user.Id);

                if (utilizador == null)
                {
                    utilizador = new Utilizadores
                    {
                        UserId = user.Id,
                        DataCriacao = DateTime.Now
                    };
                    _context.Utilizadores.Add(utilizador);
                }

                utilizador.Nome = model.Nome;
                utilizador.Telemovel = model.Telemovel;
                utilizador.Morada = model.Morada;
                utilizador.CodPostal = model.CodPostal;
                utilizador.DataAtualizacao = DateTime.Now;

                user.UserName = model.Nome;
                await _userManager.UpdateAsync(user);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Edit");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro ao atualizar o perfil.";

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    model.Email = currentUser.Email ?? "";
                    model.UserId = currentUser.Id;
                }

                return View(model);
            }
        }
    }
}