using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Villa> Villas { get; set; }
    public DbSet<VillaNumber> VillaNumbers { get; set; }
    public DbSet<LocalUser> LocalUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Villa>().HasData(
            new Villa
            {
                Id = 1,
                Name = "Royal Villa",
                Details = "The best you cant get!",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1n6ceK.img?w=1920&h=1080&q=60&m=2&f=jpg",
                Occupancy = 5,
                Rate = 5,
                Sqft = 50,
                Amenity = "",
                CreatedDate = DateTime.Now,
            },
            new Villa
            {
                Id = 2,
                Name = "Royal Villa second",
                Details = "The best you cant get!",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1n6ceK.img?w=1920&h=1080&q=60&m=2&f=jpg",
                Occupancy = 4,
                Rate = 4,
                Sqft = 40,
                Amenity = "yokk",
                CreatedDate = DateTime.Now,
            });
    }
}
