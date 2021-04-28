using GeoTagAPI_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoTagAPI_Project.Data
{
    public class GeoTagDbContext : IdentityDbContext<User>
    {
            public GeoTagDbContext(DbContextOptions<GeoTagDbContext> options)
            : base(options)
        {
        }

        public DbSet<GeoMessage> GeoMessages { get; set; }

        public async Task Seed(UserManager<User> userManager)
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();

            var user = new User { UserName = "Test", Firstname = "John", Lastname = "Doe" };
            await userManager.CreateAsync(user);

            var geoMessage = new GeoMessage
            {
                Message = "Hallo",
                Latitude = 50.2,
                Longitude = 182.6
            };

            await AddAsync(geoMessage);
            await SaveChangesAsync();
        }
    }
}
