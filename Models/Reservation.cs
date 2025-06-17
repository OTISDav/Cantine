using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CantineAPI.Models;  // Assure-toi que le namespace correspond à ton projet

namespace CantineAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        // Clé étrangère vers l'utilisateur
        public string UserId { get; set; } = string.Empty;

        // Navigation vers l'utilisateur
        public ApplicationUser User { get; set; } = null!;

        // Clé étrangère vers le menu
        public int MenuId { get; set; }

        // Navigation vers le menu
        public Menu Menu { get; set; } = null!;

        public DateTime ReservationDate { get; set; }

        // Pour éviter la nullabilité, mieux vaut initialiser à string.Empty
        public string Status { get; set; } = string.Empty; // Exemple: "Confirmée" ou "Annulée"
    }
}
