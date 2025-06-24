using System;
using CantineAPI.DTOs;
namespace CantineAPI.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; 
        public int MenuId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ReservationCreateDTO
    {
        public int MenuId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ReservationUpdateDTO
    {
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
