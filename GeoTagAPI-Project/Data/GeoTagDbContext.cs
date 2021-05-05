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
        public DbSet<Token> Tokens { get; set; }

        public async Task Seed(UserManager<User> userManager)
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();

            var user = new User { UserName = "Test", Firstname = "John", Lastname = "Doe" };
            await userManager.CreateAsync(user);

            var geoMessage = new GeoMessage
            {
                Message = new Message { Title = "Hej", Body = "Hallo", Author = user.Firstname + " " + user.Lastname },
                Latitude = 50.2,
                Longitude = 182.6
            };

            var token = new Token { Key = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"), User = user };

            await AddAsync(token);
            await AddAsync(geoMessage);
            await SaveChangesAsync();
        }
    }
}
