using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CantineAPI.Models;  // Assure-toi que le namespace correspond à ton projet

namespace CantineAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public int MenuId { get; set; }
        public Menu Menu { get; set; } = null!;
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
