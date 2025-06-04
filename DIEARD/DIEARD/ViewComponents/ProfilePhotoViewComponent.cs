using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DIEARD.Data;

namespace DIEARD.ViewComponents
{
    public class ProfilePhotoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfilePhotoViewComponent(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string? userId = null, int size = 120, bool showEditButton = false, string cssClass = "")
        {
            // Se não forneceu userId, usar o usuário atual
            if (string.IsNullOrEmpty(userId))
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                userId = currentUser?.Id;
            }

            string? fotoPerfil = null;
            string? userName = "Utilizador";

            if (!string.IsNullOrEmpty(userId))
            {
                // Buscar foto do perfil na base de dados
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                // Buscar nome do usuário
                var user = await _userManager.FindByIdAsync(userId);
                userName = user?.UserName ?? "Utilizador";

                fotoPerfil = userProfile?.FotoPerfil;
            }

            var model = new ProfilePhotoViewModel
            {
                FotoPerfil = fotoPerfil,
                UserName = userName,
                Size = size,
                CssClass = cssClass
            };

            return View(model);
        }
    }

    // ViewModel para o componente
    public class ProfilePhotoViewModel
    {
        public string? FotoPerfil { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Size { get; set; } = 120;
        public string CssClass { get; set; } = string.Empty;
    }
}