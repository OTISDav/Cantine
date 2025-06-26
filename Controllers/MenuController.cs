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
                    PhotoUrl = m.PhotoUrl,
                    Prix = m.Prix
                })
                .ToListAsync();

            return Ok(menus);
        }

        // GET /api/menu/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            var menuDto = new MenuDTO
            {
                Id = menu.Id,
                Date = menu.Date,
                PlatPrincipal = menu.PlatPrincipal,
                Dessert = menu.Dessert,
                Boisson = menu.Boisson,
                PhotoUrl = menu.PhotoUrl,
                Prix = menu.Prix
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
                // Id = menuDto.Id,
                Date = menuDto.Date,
                PlatPrincipal = menuDto.PlatPrincipal,
                Dessert = menuDto.Dessert,
                Boisson = menuDto.Boisson,
                PhotoUrl = menuDto.PhotoUrl,
                Prix = menuDto.Prix
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenu), new { id = menu.Id }, menuDto);
        }

        // PUT /api/menu/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenu(int id, MenuDTO menuDto)
        {
            if (id != menuDto.Id) 
            {
                return BadRequest("L'ID du menu dans l'URL ne correspond pas à l'ID du menu dans le corps de la requête.");
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            menu.Date = menuDto.Date;
            menu.PlatPrincipal = menuDto.PlatPrincipal;
            menu.Dessert = menuDto.Dessert;
            menu.Boisson = menuDto.Boisson;
            menu.PhotoUrl = menuDto.PhotoUrl;
            menu.Prix = menuDto.Prix;
            
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
                    throw;
            }

            return NoContent();
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

        // POST /api/menu/uploadimage
        [HttpPost("uploadimage")] 
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
            return Ok(imageUrl); 
        }

    }
}