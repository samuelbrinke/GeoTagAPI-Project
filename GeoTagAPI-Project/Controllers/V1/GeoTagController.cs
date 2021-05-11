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
            var geoMessage = await _context.GeoMessages.FirstOrDefaultAsync(g => g.Id == id);

            if (geoMessage == null)
                return NotFound();

            var geoMessageDto = new GeoMessageDto
            {
                Message = geoMessage.Body,
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude
                
            };

            return Ok(geoMessageDto);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoMessageDto>>> GetGeoMessages()
        {
            return await _context.GeoMessages
                .Select(g =>
                    new GeoMessageDto
                    {
                        Message = g.Body,
                        Longitude = g.Longitude,
                        Latitude = g.Latitude
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
                Body = geoMessageDto.Message,
                Author = $"{user.Firstname} {user.Lastname}",
                Longitude = geoMessageDto.Longitude,
                Latitude = geoMessageDto.Latitude
            };

            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            var getGeoMessage = new GeoMessageDto
            {
                Message = newGeoMessage.Body,
                Longitude = newGeoMessage.Longitude,
                Latitude = newGeoMessage.Latitude
            };

            return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, getGeoMessage);
        }

    }
}
