using DIEARD.Data;
using DIEARD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Certifique-se de adicionar esta using directive
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DIEARD.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    // Assumindo que você tem um contexto de dados onde os Diarios são armazenados
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("/search")] // Adicionando o atributo de rota para responder a /search
    public IActionResult Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return View(new List<IdentityUser>());
        }

        var users = _userManager.Users
                               .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
                               .ToList();

        return View("Search", users); // Especifica a View "Search"
    }

    public async Task<IActionResult> UserDiaries(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        // Assumindo que sua entidade Diario tem uma propriedade UserId que a relaciona ao utilizador
        var userDiaries = await _context.Diarios
                                       .Where(d => d.UserId == userId)
                                       .ToListAsync();

        ViewBag.UserName = user.UserName; // Passa o nome do utilizador para a View
        return View(userDiaries);
    }
}