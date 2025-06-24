using CantineAPI.Data;
using CantineAPI.DTOs;
using CantineAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CantineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnotationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnnotationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/annotation
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AnnotationDTO>> CreateAnnotation(AnnotationCreateDTO dto)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Utilisateur non authentifié.");

            var menu = await _context.Menus.FindAsync(dto.MenuId);
            if (menu == null)
                return BadRequest("Menu inexistant.");

            var existing = await _context.Annotations
                .FirstOrDefaultAsync(a => a.UserId == userId && a.MenuId == dto.MenuId);

            if (existing != null)
                return BadRequest("Vous avez déjà noté ce menu.");

            if (dto.Note < 1 || dto.Note > 5)
                return BadRequest("La note doit être comprise entre 1 et 5.");

            var annotation = new Annotation
            {
                UserId = userId,
                MenuId = dto.MenuId,
                Note = dto.Note,
                Commentaire = dto.Commentaire,
                CreatedAt = DateTime.UtcNow
            };

            _context.Annotations.Add(annotation);
            await _context.SaveChangesAsync();

            var resultDto = new AnnotationDTO
            {
                Id = annotation.Id,
                UserId = annotation.UserId,
                MenuId = annotation.MenuId,
                Note = annotation.Note,
                Commentaire = annotation.Commentaire,
                CreatedAt = annotation.CreatedAt
            };

            return CreatedAtAction(nameof(GetAnnotation), new { id = annotation.Id }, resultDto);
        }

        // GET api/annotation/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AnnotationDTO>> GetAnnotation(int id)
        {
            var annotation = await _context.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            bool isAdmin = User.IsInRole("Admin");

            if (annotation.UserId != userId && !isAdmin)
                return Forbid();

            var dto = new AnnotationDTO
            {
                Id = annotation.Id,
                UserId = annotation.UserId,
                MenuId = annotation.MenuId,
                Note = annotation.Note,
                Commentaire = annotation.Commentaire,
                CreatedAt = annotation.CreatedAt
            };

            return Ok(dto);
        }

        // PUT api/annotation/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnnotation(int id, AnnotationUpdateDTO dto)
        {
            var annotation = await _context.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            bool isAdmin = User.IsInRole("Admin");

            if (annotation.UserId != userId && !isAdmin)
                return Forbid();

            if (dto.Note < 1 || dto.Note > 5)
                return BadRequest("La note doit être comprise entre 1 et 5.");

            annotation.Note = dto.Note;
            annotation.Commentaire = dto.Commentaire;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/annotation/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnnotation(int id)
        {
            var annotation = await _context.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            bool isAdmin = User.IsInRole("Admin");

            if (annotation.UserId != userId && !isAdmin)
                return Forbid();

            _context.Annotations.Remove(annotation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET api/annotation/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AnnotationDTO>>> GetAllAnnotations()
        {
            var annotations = await _context.Annotations
                .Select(a => new AnnotationDTO
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    MenuId = a.MenuId,
                    Note = a.Note,
                    Commentaire = a.Commentaire,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(annotations);
        }

        // GET api/annotation/average-per-menu
        [HttpGet("average-per-menu")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAverageNotePerMenu()
        {
            var averages = await _context.Annotations
                .GroupBy(a => a.MenuId)
                .Select(g => new
                {
                    MenuId = g.Key,
                    AverageNote = g.Average(a => a.Note),
                    Count = g.Count(),
                    MenuName = _context.Menus.Where(m => m.Id == g.Key).Select(m => m.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(averages);
        }
    }
}
