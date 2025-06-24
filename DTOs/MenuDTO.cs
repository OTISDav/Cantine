using System;
using Microsoft.AspNetCore.Identity;
using CantineAPI.DTOs;
namespace CantineAPI.DTOs
{
    public class MenuDTO
    {
        public int Id { get; set; } 
        public DateTime Date { get; set; }
        public string PlatPrincipal { get; set; }
        public string Dessert { get; set; }
        public string Boisson { get; set; }
    }
}
