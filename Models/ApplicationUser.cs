using System;
using Microsoft.AspNetCore.Identity;

namespace CantineAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
