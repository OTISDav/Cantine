using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CantineAPI.Models;
using CantineAPI.DTOs;       
using CantineAPI.Data;      
using Microsoft.EntityFrameworkCore;
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

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/menu
        [HttpGet]
        public async Task<IActionResult> GetMenus()
        {
            var menus = await _context.Menus
                .Select(m => new MenuDTO
                {
                    Date = m.Date,
                    PlatPrincipal = m.PlatPrincipal,
                    Dessert = m.Dessert,
                    Boisson = m.Boisson
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
                Date = menu.Date,
                PlatPrincipal = menu.PlatPrincipal,
                Dessert = menu.Dessert,
                Boisson = menu.Boisson
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
                Date = menuDto.Date,
                PlatPrincipal = menuDto.PlatPrincipal,
                Dessert = menuDto.Dessert,
                Boisson = menuDto.Boisson
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
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            menu.Date = menuDto.Date;
            menu.PlatPrincipal = menuDto.PlatPrincipal;
            menu.Dessert = menuDto.Dessert;
            menu.Boisson = menuDto.Boisson;

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
    }
}