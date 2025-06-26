using DIEARD.Models;
using DIEARD.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Collections.Generic;

[Authorize]
public class DiarioController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    // Construtor do controlador que injeta as dependências necessárias.
    public DiarioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Verifica se dois utilizadores são amigos.
    private async Task<bool> SaoAmigos(string userId1, string userId2)
    {
        if (userId1 == userId2) return true;

        return await _context.Amizades
            .AnyAsync(a =>
                (a.UtilizadorId == userId1 && a.AmigoId == userId2) ||
                (a.UtilizadorId == userId2 && a.AmigoId == userId1));
    }

    // Verifica se um utilizador tem permissão para ver os diários de outro.
    private async Task<bool> PodeVerDiarios(string viewerId, string ownerId)
    {
        if (User.IsInRole("Administrador")) return true;
        return await SaoAmigos(viewerId, ownerId);
    }

    // GET: /Diario - Exibe os diários com base nas permissões do utilizador.
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var isAdmin = User.IsInRole("Administrador");
        IQueryable<Diarios> query;

        if (isAdmin)
        {
            query = _context.Diarios.Include(d => d.Categoria).Include(d => d.User);
        }
        else
        {
            var amizadesDoUser = await _context.Amizades
                .Where(a => a.UtilizadorId == user.Id || a.AmigoId == user.Id)
                .ToListAsync();

            var amigosIds = amizadesDoUser
                .Select(a => a.UtilizadorId == user.Id ? a.AmigoId : a.UtilizadorId)
                .ToList();

            amigosIds.Add(user.Id);

            query = _context.Diarios
                .Include(d => d.Categoria)
                .Include(d => d.User)
                .Where(d => d.UserId == user.Id);
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(d => d.CategoriaId == categoriaId.Value);
        }

        var diarios = await query
            .OrderByDescending(d => d.DataCriacao)
            .ToListAsync();

        ViewBag.Categorias = await _context.Categorias
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nome,
                Selected = c.Id == categoriaId
            })
            .ToListAsync();

        ViewBag.CategoriaSelecionada = categoriaId;
        ViewBag.IsAdmin = isAdmin;
        ViewBag.CurrentUserId = user.Id;

        return View("Index", diarios);
    }

    // GET: /Diario/Create - Exibe o formulário para criar um novo diário.
    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome");
        return View();
    }

    // POST: /Diario/Create - Processa a submissão do formulário de criação de diário.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Conteudo,CategoriaId,MoodTracker")] Diarios diario) // ALTERAÇÃO AQUI
    {
        var fieldsToRemove = new[] { "User", "UserId", "Categoria" };
        foreach (var field in fieldsToRemove)
        {
            ModelState.Remove(field);
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            diario.UserId = user.Id;
            diario.DataCriacao = DateTime.Now;
            _context.Add(diario);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocorreu um erro ao guardar o diário.");
            }
        }

        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);
        return View(diario);
    }

    // GET: /Diario/Details/5 - Exibe os detalhes de um diário específico.
    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            if (id == null || _context.Diarios == null)
            {
                return NotFound("ID inválido ou contexto não encontrado");
            }

            var diario = await _context.Diarios
                .Include(d => d.Categoria)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (diario == null)
            {
                return NotFound("Diário não encontrado");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await PodeVerDiarios(currentUser.Id, diario.UserId))
            {
                TempData["ErrorMessage"] = "Você só pode ver diários de amigos.";
                return RedirectToAction("Index");
            }

            ViewBag.IsCurrentUser = currentUser != null && currentUser.Id == diario.UserId;
            ViewBag.AuthorName = diario.User?.UserName ?? "Utilizador desconhecido";
            ViewBag.CanEdit = currentUser != null && (currentUser.Id == diario.UserId || User.IsInRole("Administrador"));

            return View(diario);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // GET: /Diario/Edit/5 - Exibe o formulário para editar um diário existente.
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var diario = await _context.Diarios.FindAsync(id);
        if (diario == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (diario.UserId != currentUser.Id && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);

        return View(diario);
    }

    // POST: /Diario/Edit/5 - Processa a submissão do formulário de edição de diário.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Conteudo,CategoriaId,MoodTracker")] Diarios diario) // ALTERAÇÃO AQUI
    {
        if (id != diario.Id)
        {
            return NotFound();
        }

        var diarioOriginal = await _context.Diarios.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (diarioOriginal == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (diarioOriginal.UserId != currentUser.Id && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        ModelState.Remove("User");
        ModelState.Remove("UserId");
        ModelState.Remove("Categoria");

        if (ModelState.IsValid)
        {
            try
            {
                diario.UserId = diarioOriginal.UserId;
                diario.DataCriacao = diarioOriginal.DataCriacao;

                _context.Update(diario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiarioExists(diario.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Details), new { id = diario.Id });
        }

        ViewBag.Categorias = new SelectList(await _context.Categorias.ToListAsync(), "Id", "Nome", diario.CategoriaId);
        return View(diario);
    }

    // POST: /Diario/Delete/5 - Processa o pedido de exclusão de um diário.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var diario = await _context.Diarios.FindAsync(id);
        if (diario == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (diario.UserId != currentUser.Id && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        try
        {
            _context.Diarios.Remove(diario);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Diário apagado com sucesso!";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Ocorreu um erro ao tentar apagar o diário.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Diario/UserDiaries - Exibe os diários de um utilizador específico.
    public async Task<IActionResult> UserDiaries(string userId, int? categoriaId, string? query)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("UserId é obrigatório");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && !await PodeVerDiarios(currentUser.Id, userId))
            {
                TempData["ErrorMessage"] = "Você só pode ver diários de amigos.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound($"Utilizador {userId} não encontrado");
            }

            var categoriasLista = await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();

            if (!categoriasLista.Any())
            {
                await SeedCategoriasIfEmpty();
                categoriasLista = await _context.Categorias
                    .OrderBy(c => c.Nome)
                    .ToListAsync();
            }

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

            var queryDiarios = _context.Diarios
                .Include(d => d.Categoria)
                .Where(d => d.UserId == userId);

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                queryDiarios = queryDiarios.Where(d => d.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrEmpty(query))
            {
                queryDiarios = queryDiarios.Where(d =>
                    d.Titulo.Contains(query) ||
                    d.Conteudo.Contains(query));
            }

            var diarios = await queryDiarios
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            ViewBag.Categorias = selectListCategorias;
            ViewBag.CategoriaSelecionada = categoriaId;
            ViewBag.CurrentSearchTerm = query;
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = userId;
            ViewBag.IsCurrentUser = currentUser != null && currentUser.Id == userId;
            ViewBag.SaoAmigos = currentUser != null && await SaoAmigos(currentUser.Id, userId);

            return View("UserDiaries", diarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // Popula a tabela de categorias com dados padrão se estiver vazia.
    private async Task SeedCategoriasIfEmpty()
    {
        try
        {
            if (!await _context.Categorias.AnyAsync())
            {
                var categoriasPadrao = new List<Categorias>
                {
                    new Categorias { Nome = "Pessoal", Descricao = "Pensamentos e experiências pessoais", DataCriacao = DateTime.Now },
                    new Categorias { Nome = "Trabalho", Descricao = "Relacionado ao trabalho e carreira", DataCriacao = DateTime.Now },
                    new Categorias { Nome = "Saúde", Descricao = "Bem-estar físico e mental", DataCriacao = DateTime.Now },
                    new Categorias { Nome = "Viagens", Descricao = "Experiências de viagem", DataCriacao = DateTime.Now },
                    new Categorias { Nome = "Família", Descricao = "Momentos em família", DataCriacao = DateTime.Now }
                };

                _context.Categorias.AddRange(categoriasPadrao);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Verifica se um diário com o ID especificado existe na base de dados.
    private bool DiarioExists(int id)
    {
        return (_context.Diarios?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}