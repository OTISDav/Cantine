using System;
using Microsoft.AspNetCore.Identity;
using CantineAPI.DTOs;


namespace CantineAPI.DTOs
{

    public class AnnotationCreateDTO
    {
        public int MenuId { get; set; }
        public int Note { get; set; } // 1 à 5
        public string? Commentaire { get; set; }
    }

    public class AnnotationUpdateDTO
    {
        public int Note { get; set; }
        public string? Commentaire { get; set; }
    }

    public class AnnotationDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int MenuId { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}