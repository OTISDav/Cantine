using System;
using CantineAPI.Models; 
using Microsoft.AspNetCore.Identity;

namespace CantineAPI.Models
{
    public class Menu
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string PlatPrincipal { get; set; } = string.Empty;

        public string Dessert { get; set; } = string.Empty;

        public string Boisson { get; set; } = string.Empty;
    }
}
