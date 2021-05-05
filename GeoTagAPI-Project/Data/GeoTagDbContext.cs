using GeoTagAPI_Project.Models;
using GeoTagAPI_Project.Models.V2;
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

            var users = new List<User>() 
            {
                new User { UserName = "Test", Firstname = "John", Lastname = "Doe" },
                new User { UserName = "Test2", Firstname = "Björn", Lastname = "Doe" }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user);
            }

            var geoMessage = new GeoMessage
            {
                Message = new Message {Title = "Hejhej", Body = "Hallo", Author = users[0].Firstname + " " + users[0].Lastname},
                Latitude = 50.2,
                Longitude = 182.6
            };

            var tokens = new List<Token> () 
            { 
                new Token { Key = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"), User = users[0] },
                new Token { Key = new Guid("00000000-0000-0000-0000-000000000000"), User = users[1] }
            };
            

            await AddRangeAsync(tokens);
            await AddAsync(geoMessage);
            await SaveChangesAsync();
        }
    }
}
