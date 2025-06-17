using System;
using CantineAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantineAPI.Models
{
    public class Annotation
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public int MenuId { get; set; }
        public Menu Menu { get; set; } = null!;

        public int Note { get; set; }  // 1 à 5

        public string? Commentaire { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
