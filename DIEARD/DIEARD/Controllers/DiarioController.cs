using DIEARD;
using DIEARD.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize]
public class DiarioController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DiarioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(string Titulo, string Conteudo)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var novoDiario = new Diario
            {
                Titulo = Titulo,
                Conteudo = Conteudo,
                UserId = user.Id
            };

            _context.Diarios.Add(novoDiario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    public IActionResult Index()
    {
        return View();
    }
}