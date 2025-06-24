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
    [Authorize] // Tous les endpoints demandent une authentification
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            IQueryable<Reservation> query = _context.Reservations;

            // Si pas admin, ne retourne que les réservations de l'utilisateur
            if (!isAdmin)
            {
                query = query.Where(r => r.UserId == userId);
            }

            var reservations = await query
                .Include(r => r.User)
                .Select(r => new ReservationDTO
                {
                    Id = r.Id,
                    UserName = r.User.FullName,
                    UserId = r.UserId,
                    MenuId = r.MenuId,
                    ReservationDate = r.ReservationDate,
                    Status = r.Status
                }).ToListAsync();

            return Ok(reservations);
        }

        // GET api/reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            // Si pas admin, vérifier que la réservation appartient à l'utilisateur
            if (!isAdmin && reservation.UserId != userId)
                return Forbid();

            var dto = new ReservationDTO
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                MenuId = reservation.MenuId,
                ReservationDate = reservation.ReservationDate,
                Status = reservation.Status
            };

            return Ok(dto);
        }

        // POST api/reservation
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(ReservationCreateDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservation = new Reservation
            {
                UserId = userId, // prend l'utilisateur connecté
                MenuId = dto.MenuId,
                ReservationDate = dto.ReservationDate,
                Status = dto.Status
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var resultDto = new ReservationDTO
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                MenuId = reservation.MenuId,
                ReservationDate = reservation.ReservationDate,
                Status = reservation.Status
            };

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, resultDto);
        }

        // PUT api/reservation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationUpdateDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            // Si pas admin, vérifier que l'utilisateur possède la réservation
            if (!isAdmin && reservation.UserId != userId)
                return Forbid();

            reservation.ReservationDate = dto.ReservationDate;
            reservation.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/reservation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
