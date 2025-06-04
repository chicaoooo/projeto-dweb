using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DIEARD.Models;
using DIEARD.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DIEARD.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: /Profile/Editar
        public async Task<IActionResult> Editar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Buscar perfil do utilizador na nova tabela UserProfile
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            var model = new ProfileViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                FotoPerfil = userProfile?.FotoPerfil
            };

            return View(model);
        }

        // POST: /Profile/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return NotFound();

                // Atualizar dados básicos do Identity User
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }

                // Buscar ou criar perfil estendido do utilizador
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (userProfile == null)
                {
                    userProfile = new UserProfile
                    {
                        UserId = user.Id,
                        DataCriacao = DateTime.Now
                    };
                    _context.UserProfiles.Add(userProfile);
                }

                // Atualizar dados do perfil estendido
                userProfile.DataAtualizacao = DateTime.Now;

                // Processar upload da foto
                if (model.FotoPerfilFile != null && model.FotoPerfilFile.Length > 0)
                {
                    // Validar tipo de arquivo
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(model.FotoPerfilFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("FotoPerfilFile", "Por favor, selecione uma imagem válida (JPG, PNG ou GIF).");
                        return View(model);
                    }

                    // Validar tamanho (máximo 5MB)
                    if (model.FotoPerfilFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("FotoPerfilFile", "A imagem não pode ser maior que 5MB.");
                        return View(model);
                    }

                    // Converter para Base64 e guardar
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.FotoPerfilFile.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);
                        userProfile.FotoPerfil = $"data:{model.FotoPerfilFile.ContentType};base64,{base64String}";
                    }

                    _logger.LogInformation($"Foto do perfil atualizada para o utilizador {user.Id}");
                }
                else if (model.RemoverFoto)
                {
                    // Remover foto do perfil
                    userProfile.FotoPerfil = null;
                    _logger.LogInformation($"Foto do perfil removida para o utilizador {user.Id}");
                }

                // Salvar alterações na base de dados
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
                return RedirectToAction("Editar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuário {UserId}", _userManager.GetUserId(User));
                TempData["ErrorMessage"] = "Ocorreu um erro ao atualizar o perfil. Tente novamente.";
                return View(model);
            }
        }

        // POST: /Profile/RemoverFoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverFoto()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return NotFound();

                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (userProfile != null)
                {
                    userProfile.FotoPerfil = null;
                    userProfile.DataAtualizacao = DateTime.Now;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Foto do perfil removida para o usuário {user.Id}");
                }

                TempData["SuccessMessage"] = "Foto do perfil removida com sucesso!";
                return RedirectToAction("Editar");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover foto do perfil do usuário {UserId}", _userManager.GetUserId(User));
                TempData["ErrorMessage"] = "Ocorreu um erro ao remover a foto. Tente novamente.";
                return RedirectToAction("Editar");
            }
        }

        // GET: /Profile/Index (visualização do perfil)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            var model = new ProfileViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                FotoPerfil = userProfile?.FotoPerfil
            };

            return View(model);
        }
    }
}