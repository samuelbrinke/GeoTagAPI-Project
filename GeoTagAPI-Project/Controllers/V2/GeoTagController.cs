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
using GeoTagAPI_Project.Dtos.V2;
using Microsoft.AspNetCore.Identity;

namespace GeoTagAPI_Project.Controllers.V2
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("2.0")]
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
        public async Task<ActionResult<GetGeoMessageDto>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.FirstOrDefaultAsync(g => g.Id == id);

            if (geoMessage == null)
                return NotFound();

            var geoMessageDto = new GetGeoMessageDto
            {
                Message = new GetMessageDto { Title = geoMessage.Title, Body = geoMessage.Body, Author = geoMessage.Author },
                Longitude = geoMessage.Longitude,
                Latitude = geoMessage.Latitude
            };

            return Ok(geoMessageDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetGeoMessageDto>>> GetGeoMessagesQuery([FromQuery] double minLon, [FromQuery] double minLat, [FromQuery] double maxLon, [FromQuery] double maxLat)
        {
            var geoMessages = await _context.GeoMessages
                .Select(g =>
                    new GetGeoMessageDto
                    {
                        Message = new GetMessageDto { Title = g.Title, Body = g.Body, Author = g.Author },
                        Longitude = g.Longitude,
                        Latitude = g.Latitude
                    }
                )
                .ToListAsync();

            if (Request.Query.ContainsKey("minLon") && Request.Query.ContainsKey("minLat") && Request.Query.ContainsKey("maxLon") && Request.Query.ContainsKey("maxLat"))
                geoMessages = geoMessages.Where(g =>
                    g.Longitude > minLon && g.Longitude < maxLon &&
                    g.Latitude > minLat && g.Latitude < maxLat
                ).ToList();

            return geoMessages;
        }


        /// <summary>
        /// Creates a new Geo-Message.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GetGeoMessageDto>> CreateGeoMessage(AddGeoMessageDto addGeoMessage)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var newGeoMessage = new GeoMessage
            {
                Title = addGeoMessage.Message.Title,
                Body = addGeoMessage.Message.Body,
                Author = $"{user.Firstname} {user.Lastname}",
                Longitude = addGeoMessage.Longitude,
                Latitude = addGeoMessage.Latitude
            };

            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            var getGeoMessage = new GetGeoMessageDto
            {
                Message = new GetMessageDto { Title = newGeoMessage.Title, Body = newGeoMessage.Body, Author = newGeoMessage.Author },
                Longitude = newGeoMessage.Longitude,
                Latitude = newGeoMessage.Latitude
            };
            return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, getGeoMessage);
        }
    }
}
