using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CantineAPI.Models;
using CantineAPI.DTOs;
using CantineAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting; 
using System.Threading.Tasks;
using System.Linq;
using System;


namespace CantineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; 

        public MenuController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET /api/menu
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var menus = await _context.Menus
                .Select(m => new MenuDTO
                {
                    Id = m.Id,
                    Date = m.Date,
                    PlatPrincipal = m.PlatPrincipal,
                    Dessert = m.Dessert,
                    Boisson = m.Boisson,
                    // --- AJOUT DE PhotoUrl et Prix lors de la récupération ---
                    PhotoUrl = m.PhotoUrl,
                    Prix = m.Prix
                    // ----------------------------------------------------
                })
                .ToListAsync();

            return Ok(menus);
        }

        // GET /api/menu/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
        {
            // Pas besoin de 'Select' pour un FindAsync, mappez directement l'objet trouvé au DTO
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            var menuDto = new MenuDTO
            {
                Id = menu.Id,
                Date = menu.Date,
                PlatPrincipal = menu.PlatPrincipal,
                Dessert = menu.Dessert,
                Boisson = menu.Boisson,
                // --- AJOUT DE PhotoUrl et Prix pour un menu unique ---
                PhotoUrl = menu.PhotoUrl,
                Prix = menu.Prix
                // --------------------------------------------------
            };

            return Ok(menuDto);
        }

        // POST /api/menu
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenu(MenuDTO menuDto)
        {
            var menu = new Menu
            {
                // L'ID ne doit généralement pas être défini ici, la base de données le génère
                // Si votre base de données gère les IDs automatiquement (ce qui est courant), supprimez 'Id = menuDto.Id'
                // Id = menuDto.Id,
                Date = menuDto.Date,
                PlatPrincipal = menuDto.PlatPrincipal,
                Dessert = menuDto.Dessert,
                Boisson = menuDto.Boisson,
                // --- AJOUT DE PhotoUrl et Prix lors de la création ---
                PhotoUrl = menuDto.PhotoUrl,
                Prix = menuDto.Prix
                // --------------------------------------------------
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            // Retourne le DTO qui a été utilisé pour la création, ou un DTO remappé si l'ID est généré par la DB
            return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menuDto);
        }

        // PUT /api/menu/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenu(int id, MenuDTO menuDto)
        {
            if (id != menuDto.Id) // Vérification de cohérence entre l'ID de l'URL et l'ID du corps
            {
                return BadRequest("L'ID du menu dans l'URL ne correspond pas à l'ID du menu dans le corps de la requête.");
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            // Mettez à jour toutes les propriétés modifiables du modèle de base de données avec les valeurs du DTO
            menu.Date = menuDto.Date;
            menu.PlatPrincipal = menuDto.PlatPrincipal;
            menu.Dessert = menuDto.Dessert;
            menu.Boisson = menuDto.Boisson;
            // --- AJOUT DE PhotoUrl et Prix lors de la mise à jour ---
            menu.PhotoUrl = menuDto.PhotoUrl;
            menu.Prix = menuDto.Prix;
            // ------------------------------------------------------

            _context.Entry(menu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Menus.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw; // Lance l'exception si c'est un autre type d'erreur
            }

            return NoContent(); // 204 No Content est une réponse standard pour les PUT réussis
        }

        // DELETE /api/menu/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("uploadimage")] // URL: /api/menu/uploadimage
        [Authorize(Roles = "Admin")]

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadMenuImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Aucun fichier n'a été envoyé.");

            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "menus");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string imageUrl = $"/images/menus/{uniqueFileName}";
            return Ok(imageUrl); // Renvoie l'URL pour le frontend
        }

    }
}