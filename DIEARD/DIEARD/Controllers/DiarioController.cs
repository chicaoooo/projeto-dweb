using DIEARD.Models;
using DIEARD.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
[Route("[controller]")] // Rota base para este controller será /Diario
public class DiarioController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<DiarioController> _logger;

    public DiarioController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILoggerFactory loggerFactory)
    {
        _context = context;
        _userManager = userManager;
        _logger = loggerFactory.CreateLogger<DiarioController>();
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        var diariosDoUsuario = await _context.Diarios
            .Where(d => d.UserId == user.Id)
            .ToListAsync();
        return View(diariosDoUsuario);
    }

    [HttpGet("Criar")]
    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost("Criar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar([Bind("Titulo", "Conteudo")] Diario diario)
    {
        _logger.LogInformation("A tentar criar um novo diário.");
        // Remover erros de validação para User e UserId, pois serão definidos no servidor
        ModelState.Remove("User");
        ModelState.Remove("UserId");

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Utilizador não encontrado ao criar o diário.");
                return NotFound();
            }
            diario.UserId = user.Id;
            diario.DataCriacao = DateTime.Now;
            _context.Add(diario);
            _logger.LogInformation("Diário adicionado ao contexto. A chamar SaveChangesAsync.");
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Diário guardado com sucesso.");
                return RedirectToAction(nameof(VisualizarLista));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao guardar o diário: {ex.Message}");
                ModelState.AddModelError("", "Ocorreu um erro ao guardar o diário.");
                return View(diario);
            }
        }
        _logger.LogWarning($"ModelState inválido ao criar o diário. Erros: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
        return View(diario);
    }

    // GET: /Diario/Visualizar (Lista de diários)
    [HttpGet("Visualizar")]
    public async Task<IActionResult> VisualizarLista() // Renomeei para VisualizarLista
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }
        var diariosDoUsuario = await _context.Diarios
            .Where(d => d.UserId == user.Id)
            .ToListAsync();
        return View("Visualizar", diariosDoUsuario); // Retorna a view para a lista
    }

    // GET: /Diario/Visualizar/5 (Detalhes de um diário)
    [HttpGet("Visualizar/{id:int}")]
    public async Task<IActionResult> Visualizar(int? id)
    {
        if (id == null || _context.Diarios == null)
        {
            return NotFound();
        }
        var diario = await _context.Diarios
            .FirstOrDefaultAsync(m => m.Id == id);
        if (diario == null)
        {
            return NotFound();
        }
        return View("VisualizarDetalhe", diario); // Retorna a view para os detalhes
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int? id)
    {
        if (id == null || _context.Diarios == null)
        {
            return NotFound();
        }

        var diario = await _context.Diarios.FindAsync(id);
        if (diario == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || diario.UserId != user.Id)
        {
            return Forbid(); // Ou NotFound(), dependendo de como você quer lidar com isso
        }

        return View(diario);
    }

    [HttpPost("Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Diario diario)
    {
        _logger.LogInformation($"Tentativa de edição do diário com ID: {id}");
        _logger.LogInformation($"Dados recebidos - Título: {diario.Titulo}, Conteúdo: {diario.Conteudo}, UserId: {diario.UserId}");

        if (id != diario.Id)
        {
            _logger.LogWarning($"ID da rota ({id}) não corresponde ao ID do diário ({diario.Id}).");
            return NotFound();
        }

        // Explicitamente remover a validação para a propriedade User
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Utilizador não encontrado.");
                return NotFound();
            }

            var diarioOriginal = await _context.Diarios.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (diarioOriginal == null || diarioOriginal.UserId != user.Id)
            {
                _logger.LogWarning($"Tentativa de edição de diário ({id}) por utilizador não autorizado ({user?.Id}).");
                return Forbid(); // Ou NotFound()
            }

            try
            {
                diario.UserId = user.Id; // Garante que o UserId seja o do utilizador logado
                diario.DataCriacao = DateTime.Now;
                _context.Update(diario);
                _logger.LogInformation("Chamando SaveChangesAsync.");
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Diário com ID: {id} editado com sucesso.");
                return RedirectToAction(nameof(VisualizarLista)); // Redireciona para a lista após salvar
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Erro de concorrência ao editar o diário {id}: {ex.Message}");
                if (!DiarioExists(diario.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao editar o diário {id}: {ex.Message}");
                return View(diario);
            }
        }
        else
        {
            _logger.LogWarning($"ModelState inválido ao editar o diário {id}. Erros: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
        }
        return View(diario); // Se o ModelState não for válido, retorna a view com erros
    }

    [HttpPost("Apagar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApagarConfirmado(int id)
    {
        if (_context.Diarios == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Diarios' is null.");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var diario = await _context.Diarios.FindAsync(id);
        if (diario == null || diario.UserId != user.Id)
        {
            return Forbid(); // Ou NotFound()
        }

        if (diario != null)
        {
            _context.Diarios.Remove(diario);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(VisualizarLista));
    }

    private bool DiarioExists(int id)
    {
        return (_context.Diarios?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}