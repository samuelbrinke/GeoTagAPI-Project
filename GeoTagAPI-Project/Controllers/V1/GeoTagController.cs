using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoTagAPI_Project.Models;
using GeoTagAPI_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeoTagAPI_Project.Dtos.V1;
using Microsoft.AspNetCore.Identity;

namespace GeoTagAPI_Project.Controllers.V1
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("1.0")]
    public class GeoTagController : ControllerBase
    {
        private readonly GeoTagDbContext _context;
        private readonly UserManager<User> _userManager;
        public GeoTagController(GeoTagDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeoMessageDto>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.Include(g => g.Message).FirstOrDefaultAsync(g => g.Id == id);

            if (geoMessage == null)
                return NotFound();

            var geoMessageDto = new GeoMessageDto
            {
                Id = geoMessage.Id,
                Message = geoMessage.Message.Body,
                Latitude = geoMessage.Latitude,
                Longitude = geoMessage.Longitude
            };

            return Ok(geoMessageDto);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoMessageDto>>> GetGeoMessages()
        {
            return await _context.GeoMessages
                .Include(g => g.Message)
                .Select(g =>
                    new GeoMessageDto
                    {
                        Id = g.Id,
                        Message = g.Message.Body,
                        Latitude = g.Latitude,
                        Longitude = g.Longitude
                    }
                )
                .ToListAsync();
        }


        /// <summary>
        /// Creates a new Geo-Message.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GeoMessageDto>> CreateGeoMessage(GeoMessageDto geoMessageDto)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var newGeoMessage = new GeoMessage
            {
                Message = new Message
                {
                    Body = geoMessageDto.Message,
                    Author = $"{user.Firstname} {user.Lastname}"
                },
                Latitude = geoMessageDto.Latitude,
                Longitude = geoMessageDto.Longitude
            };

            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, new { newGeoMessage.Id, Message = newGeoMessage.Message.Body, newGeoMessage.Longitude, newGeoMessage.Latitude });
        }

    }
}
