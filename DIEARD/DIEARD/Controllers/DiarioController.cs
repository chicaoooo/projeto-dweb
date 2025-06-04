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
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
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

    // GET: /Diario
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var query = _context.Diarios
            .Include(d => d.Categoria)
            .Where(d => d.UserId == user.Id);

        // Aplicar filtro por categoria se especificado
        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(d => d.CategoriaId == categoriaId.Value);
        }

        var diariosDoUsuario = await query
            .OrderByDescending(d => d.DataCriacao)
            .ToListAsync();

        // Carregar categorias para o dropdown
        ViewBag.Categorias = await _context.Categorias
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome,
                Selected = c.Id == categoriaId
            })
            .ToListAsync();

        ViewBag.CategoriaSelecionada = categoriaId;

        return View("Visualizar", diariosDoUsuario);
    }

    // GET: /Diario/Criar
    public async Task<IActionResult> Criar()
    {
        // Carregar categorias para o dropdown
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome");
        return View();
    }

    // POST: /Diario/Criar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar([Bind("Titulo", "Conteudo", "CategoriaId")] Diario diario)
    {
        _logger.LogInformation("A tentar criar um novo diário.");

        // Remover erros de validação para propriedades que serão definidas no servidor
        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("Categoria");

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
                return RedirectToAction(nameof(Visualizar));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao guardar o diário: {ex.Message}");
                ModelState.AddModelError("", "Ocorreu um erro ao guardar o diário.");
            }
        }

        // Recarregar categorias em caso de erro
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);
        _logger.LogWarning($"ModelState inválido ao criar o diário. Erros: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
        return View(diario);
    }

    // GET: /Diario/Visualizar (Lista de diários com filtro)
    public async Task<IActionResult> Visualizar(int? categoriaId)
    {
        return await Index(categoriaId);
    }

    // GET: /Diario/Detalhes/5 
    [AllowAnonymous] // Permite acesso anônimo para visualizar detalhes
    public async Task<IActionResult> Detalhes(int? id)
    {
        try
        {
            if (id == null || _context.Diarios == null)
            {
                return NotFound("ID inválido ou contexto não encontrado");
            }

            var diario = await _context.Diarios
                .Include(d => d.Categoria)
                .Include(d => d.User) // Incluir dados do usuário para mostrar o autor
                .FirstOrDefaultAsync(m => m.Id == id);

            if (diario == null)
            {
                return NotFound("Diário não encontrado");
            }

            // Verificar se o usuário atual é o dono do diário
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.IsCurrentUser = currentUser != null && currentUser.Id == diario.UserId;
            ViewBag.AuthorName = diario.User?.UserName ?? "Utilizador desconhecido";

            _logger.LogInformation($"Detalhes do diário {id} visualizados por {currentUser?.UserName ?? "visitante anônimo"}");

            return View(diario);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar detalhes do diário {id}: {ex.Message}");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // GET: /Diario/Editar/5
    [Authorize]
    public async Task<IActionResult> Editar(int? id)
    {
        try
        {
            if (id == null || _context.Diarios == null)
            {
                return NotFound("ID inválido");
            }

            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning($"Tentativa de editar diário {id} por utilizador não autenticado");
                return Unauthorized("Utilizador não autenticado");
            }

            
            var diario = await _context.Diarios
                .Include(d => d.Categoria)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diario == null)
            {
                _logger.LogWarning($"Tentativa de editar diário inexistente: {id}");
                return NotFound("Diário não encontrado");
            }

            
            if (diario.UserId != currentUser.Id)
            {
                _logger.LogWarning($"TENTATIVA DE VIOLAÇÃO DE SEGURANÇA: Utilizador {currentUser.Id} ({currentUser.UserName}) tentou editar diário {id} que pertence ao usuário {diario.UserId}");
                return Forbid("Não tem permissão para editar este diário");
            }

            
            ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);

            _logger.LogInformation($"Formulário de edição carregado para diário {id} pelo utilizador {currentUser.Id}");

            return View(diario);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar formulário de edição para diário {id}: {ex.Message}");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // POST: /Diario/Editar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, [Bind("Id", "Titulo", "Conteudo", "CategoriaId", "UserId")] Diario diario)
    {
        _logger.LogInformation($"Tentativa de edição do diário com ID: {id}");

        if (id != diario.Id)
        {
            _logger.LogWarning($"ID da rota ({id}) não corresponde ao ID do diário ({diario.Id}).");
            return NotFound();
        }

        // Remover validações para propriedades de navegação
        ModelState.Remove("User");
        ModelState.Remove("Categoria");

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
                return Forbid();
            }

            try
            {
                diario.UserId = user.Id;
                diario.DataCriacao = diarioOriginal.DataCriacao; // Manter data original
                _context.Update(diario);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Diário com ID: {id} editado com sucesso.");
                return RedirectToAction(nameof(Visualizar));
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
                ModelState.AddModelError("", "Ocorreu um erro ao editar o diário.");
            }
        }

        // Recarregar categorias em caso de erro
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);
        _logger.LogWarning($"ModelState inválido ao editar o diário {id}.");
        return View(diario);
    }

    // POST: /Diario/Apagar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Apagar(int id)
    {
        try
        {
            if (_context.Diarios == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Diarios' is null.");
            }

            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning($"Tentativa de apagar diário {id} por utilizador não autenticado");
                return Unauthorized("Utilizador não autenticado");
            }

            
            var diario = await _context.Diarios.FindAsync(id);
            if (diario == null)
            {
                _logger.LogWarning($"Tentativa de apagar diário inexistente: {id}");
                return NotFound("Diário não encontrado");
            }

            
            if (diario.UserId != currentUser.Id)
            {
                _logger.LogWarning($"TENTATIVA DE VIOLAÇÃO DE SEGURANÇA: Utilizador {currentUser.Id} ({currentUser.UserName}) tentou apagar diário {id} que pertence ao usuário {diario.UserId}");
                return Forbid("Não tem permissão para apagar este diário");
            }

            
            _context.Diarios.Remove(diario);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Diário {id} apagado com sucesso pelo utilizador {currentUser.Id} ({currentUser.UserName})");

            return RedirectToAction(nameof(Visualizar));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao apagar diário {id}: {ex.Message}");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // GET: /Diario/UserDiaries
    [AllowAnonymous]
    public async Task<IActionResult> UserDiaries(string userId, int? categoriaId, string? query)
    {
        try
        {
            // Verificar parâmetros
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("UserId é obrigatório");
            }

            // Verificar usuário
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound($"Utilizador {userId} não encontrado");
            }

            // Carregar categorias
            var categoriasLista = await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();

            // Se não existem categorias, criar as padrão
            if (categoriasLista.Count == 0)
            {
                await SeedCategoriasIfEmpty();
                categoriasLista = await _context.Categorias
                    .OrderBy(c => c.Nome)
                    .ToListAsync();
            }

            // Criar SelectList para as categorias
            var selectListCategorias = new SelectList(
                categoriasLista.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome,
                    Selected = categoriaId.HasValue && c.Id == categoriaId.Value
                }),
                "Value",
                "Text",
                categoriaId?.ToString()
            );

            // Carregar diários
            var queryDiarios = _context.Diarios
                .Include(d => d.Categoria)
                .Where(d => d.UserId == userId);

            // Aplicar filtro por categoria se especificado
            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                queryDiarios = queryDiarios.Where(d => d.CategoriaId == categoriaId.Value);
            }

            // Aplicar filtro de pesquisa se especificado
            if (!string.IsNullOrEmpty(query))
            {
                queryDiarios = queryDiarios.Where(d =>
                    d.Titulo.Contains(query) ||
                    d.Conteudo.Contains(query));
            }

            var diarios = await queryDiarios
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            // Configurar ViewBag
            ViewBag.Categorias = selectListCategorias;
            ViewBag.CategoriasDebug = categoriasLista;
            ViewBag.CategoriasCount = categoriasLista.Count;
            ViewBag.CategoriaSelecionada = categoriaId;
            ViewBag.CurrentSearchTerm = query;
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = userId;

            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.IsCurrentUser = currentUser != null && currentUser.Id == userId;

            return View("UserDiaries", diarios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro em UserDiaries: {ex.Message}");
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // Método auxiliar para criar categorias padrão
    private async Task SeedCategoriasIfEmpty()
    {
        try
        {
            var existemCategorias = await _context.Categorias.AnyAsync();
            if (!existemCategorias)
            {
                var categoriasPadrao = new List<Categoria>
                {
                    new Categoria { Nome = "Pessoal", Descricao = "Pensamentos e experiências pessoais", DataCriacao = DateTime.Now },
                    new Categoria { Nome = "Trabalho", Descricao = "Relacionado ao trabalho e carreira", DataCriacao = DateTime.Now },
                    new Categoria { Nome = "Saúde", Descricao = "Bem-estar físico e mental", DataCriacao = DateTime.Now },
                    new Categoria { Nome = "Viagens", Descricao = "Experiências de viagem", DataCriacao = DateTime.Now },
                    new Categoria { Nome = "Família", Descricao = "Momentos em família", DataCriacao = DateTime.Now }
                };

                _context.Categorias.AddRange(categoriasPadrao);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro no seed das categorias: {ex.Message}");
            throw;
        }
    }

    private bool DiarioExists(int id)
    {
        return (_context.Diarios?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    // GET: /Diario/VisualizarLista
    [AllowAnonymous]
    public async Task<IActionResult> VisualizarLista(string userId, int? categoriaId, string? query)
    {
        try
        {
            // Se não há userId, buscar o usuário atual
            if (string.IsNullOrEmpty(userId))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                userId = currentUser.Id;
            }

            // Verificar se o usuário existe
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound($"Utilizador não encontrado");
            }

            // Carregar categorias
            var categoriasLista = await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();

            // Se não existem categorias, criar as padrão
            if (categoriasLista.Count == 0)
            {
                await SeedCategoriasIfEmpty();
                categoriasLista = await _context.Categorias
                    .OrderBy(c => c.Nome)
                    .ToListAsync();
            }

            // Criar SelectList para as categorias
            var selectListCategorias = new SelectList(
                categoriasLista,
                "Id",
                "Nome",
                categoriaId
            );

            // Carregar diários
            var queryDiarios = _context.Diarios
                .Include(d => d.Categoria)
                .Where(d => d.UserId == userId);

            // Aplicar filtro por categoria se especificado
            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                queryDiarios = queryDiarios.Where(d => d.CategoriaId == categoriaId.Value);
            }

            // Aplicar filtro de pesquisa se especificado
            if (!string.IsNullOrEmpty(query))
            {
                queryDiarios = queryDiarios.Where(d =>
                    d.Titulo.Contains(query) ||
                    d.Conteudo.Contains(query));
            }

            var diarios = await queryDiarios
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            // Configurar ViewBag
            ViewBag.Categorias = selectListCategorias;
            ViewBag.CategoriasDebug = categoriasLista;
            ViewBag.CategoriasCount = categoriasLista.Count;
            ViewBag.CategoriaSelecionada = categoriaId;
            ViewBag.CurrentSearchTerm = query;
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = userId;

            var currentLoggedUser = await _userManager.GetUserAsync(User);
            ViewBag.IsCurrentUser = currentLoggedUser != null && currentLoggedUser.Id == userId;

            // Usar a view UserDiaries que já funciona
            return View("UserDiaries", diarios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro em VisualizarLista: {ex.Message}");
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}