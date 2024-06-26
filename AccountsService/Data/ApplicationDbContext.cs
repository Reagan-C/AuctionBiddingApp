﻿using AccountsService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                },
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
            modelBuilder.Entity<RefreshToken>()
               .HasOne(r => r.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(r => r.UserId);
        }
    }
}
