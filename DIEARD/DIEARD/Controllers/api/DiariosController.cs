using DIEARD.Data;
using DIEARD.Models;
using DIEARD.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DIEARD.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DiariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const string AdminUserId = "admin";

        public DiariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Método auxiliar para verificar se o utilizador é admin pelo ID
        private bool IsAdmin(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId == AdminUserId;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiarioDto>>> GetDiarios(int? categoriaId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = IsAdmin(User);

            var query = _context.Diarios
                .Include(d => d.Categoria)
                .Include(d => d.User)
                .AsQueryable();

            // Administradores conseguem ver todos os diários
            if (!isAdmin)
            {
                // Utilizadores normais veem apenas seus diários e de amigos
                var amigosIds = await _context.Amizades
                    .Where(a => a.UtilizadorId == userId || a.AmigoId == userId)
                    .Select(a => a.UtilizadorId == userId ? a.AmigoId : a.UtilizadorId)
                    .ToListAsync();

                amigosIds.Add(userId);
                query = query.Where(d => amigosIds.Contains(d.UserId));
            }

            // Filtro de categoria aplicado a todos
            if (categoriaId.HasValue)
            {
                query = query.Where(d => d.CategoriaId == categoriaId.Value);
            }

            var diarios = await query
                .OrderByDescending(d => d.DataCriacao)
                .Select(d => new DiarioDto
                {
                    Id = d.Id,
                    Titulo = d.Titulo,
                    Conteudo = d.Conteudo,
                    Categoria = d.Categoria.Nome,
                    Autor = d.User.UserName,
                    DataCriacao = d.DataCriacao,
                    MoodTracker = d.MoodTracker
                })
                .ToListAsync();

            return Ok(diarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiarioDto>> GetDiario(int id)
        {
            var diario = await _context.Diarios
                .Include(d => d.Categoria)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diario == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = IsAdmin(User);

            // Admin tem acesso a tudo, outros utilizadores precisam ser donos ou amigos
            if (!isAdmin && diario.UserId != userId)
            {
                var saoAmigos = await _context.Amizades
                    .AnyAsync(a =>
                        (a.UtilizadorId == userId && a.AmigoId == diario.UserId) ||
                        (a.UtilizadorId == diario.UserId && a.AmigoId == userId));

                if (!saoAmigos)
                    return Forbid();
            }

            return Ok(new DiarioDto
            {
                Id = diario.Id,
                Titulo = diario.Titulo,
                Conteudo = diario.Conteudo,
                Categoria = diario.Categoria.Nome,
                Autor = diario.User.UserName,
                DataCriacao = diario.DataCriacao,
                MoodTracker = diario.MoodTracker
            });
        }

        [HttpPost]
        public async Task<ActionResult<DiarioDto>> CreateDiario([FromBody] DiarioCreateDto diarioDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var diario = new Diarios
            {
                Titulo = diarioDto.Titulo,
                Conteudo = diarioDto.Conteudo,
                CategoriaId = diarioDto.CategoriaId,
                UserId = userId,
                DataCriacao = DateTime.Now,
                MoodTracker = diarioDto.MoodTracker
            };

            _context.Diarios.Add(diario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDiario), new { id = diario.Id }, diarioDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiario(int id, [FromBody] DiarioUpdateDto diarioDto)
        {
            var diario = await _context.Diarios.FindAsync(id);
            if (diario == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = IsAdmin(User);

            // Apenas admin ou dono podem editar
            if (diario.UserId != userId && !isAdmin)
                return Forbid();

            diario.Titulo = diarioDto.Titulo;
            diario.Conteudo = diarioDto.Conteudo;
            diario.CategoriaId = diarioDto.CategoriaId;
            diario.MoodTracker = diarioDto.MoodTracker;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiario(int id)
        {
            var diario = await _context.Diarios.FindAsync(id);
            if (diario == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = IsAdmin(User);

            // Apenas admin ou dono podem excluir
            if (diario.UserId != userId && !isAdmin)
                return Forbid();

            _context.Diarios.Remove(diario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Endpoint para depuração
        [HttpGet("debug-info")]
        public IActionResult GetDebugInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = IsAdmin(User);

            return Ok(new
            {
                UserId = userId,
                IsAdmin = isAdmin,
                AdminUserId = AdminUserId,
                CurrentTime = DateTime.Now
            });
        }
    }
}