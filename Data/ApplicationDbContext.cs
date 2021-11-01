using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using FilmsCatalog.Models;

namespace FilmsCatalog.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Movie>()
        //         .HasOne<User>(m => m.Creator)
        //         .WithMany(u => u.Movies)
        //         .HasForeignKey(m => m.CreatorIdentityName);
        // }

        public DbSet<Movie> Movie { get; set; }
    }
}
