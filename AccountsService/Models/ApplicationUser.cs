﻿using Microsoft.AspNetCore.Identity;

namespace AccountsService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [PersonalData]
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
