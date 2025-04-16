﻿using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
    }
}
