using CantineAPI.Data;
using CantineAPI.DTOs;
using CantineAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


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
        // Un utilisateur note un menu (note de 1 à 5)
        [HttpPost]
        [Authorize] // Utilisateur connecté
        public async Task<ActionResult<AnnotationDTO>> CreateAnnotation(AnnotationCreateDTO dto)
        {
            // Récupérer l'id utilisateur connecté (ici on suppose User.Identity.Name contient l'ID)
            string userId = User.Identity?.Name ?? "anonymous";

            // Vérifier si l'utilisateur a déjà noté ce menu (une seule note par utilisateur/menu)
            var existing = await _context.Annotations
                .FirstOrDefaultAsync(a => a.UserId == userId && a.MenuId == dto.MenuId);

            if (existing != null)
            {
                return BadRequest("Vous avez déjà noté ce menu.");
            }

            if (dto.Note < 1 || dto.Note > 5)
            {
                return BadRequest("La note doit être comprise entre 1 et 5.");
            }

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

            // Optionnel : vérifier que l'utilisateur est propriétaire ou admin
            string userId = User.Identity?.Name ?? "";
            bool isAdmin = User.IsInRole("Admin");
            if (annotation.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

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
        // Modifier sa note/commentaire
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnnotation(int id, AnnotationUpdateDTO dto)
        {
            var annotation = await _context.Annotations.FindAsync(id);
            if (annotation == null) return NotFound();

            string userId = User.Identity?.Name ?? "";
            bool isAdmin = User.IsInRole("Admin");
            if (annotation.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            if (dto.Note < 1 || dto.Note > 5)
            {
                return BadRequest("La note doit être comprise entre 1 et 5.");
            }

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

            string userId = User.Identity?.Name ?? "";
            bool isAdmin = User.IsInRole("Admin");
            if (annotation.UserId != userId && !isAdmin)
            {
                return Forbid();
            }

            _context.Annotations.Remove(annotation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET api/annotation/all
        // L'admin voit toutes les annotations
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
        // L'admin récupère la moyenne des notes par menu
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
                    // Optionnel: ajouter le nom du menu (requiert une navigation vers Menu)
                    MenuName = _context.Menus.Where(m => m.Id == g.Key).Select(m => m.Name).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(averages);
        }
    }
}
