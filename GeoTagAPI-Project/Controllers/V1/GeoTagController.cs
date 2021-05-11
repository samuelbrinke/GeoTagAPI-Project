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

        /// <summary>
        /// Retrieves a specific Geo-Message by unique id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Geo-Message found!</response>
        /// <response code="404">Failed to find Geo-Message</response>
        /// <returns>This returns a Geo-Message</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Retrieves all Geo-Messages
        /// </summary>
        /// <response code="200">Geo-Messages found!</response>
        /// <response code="404">Failed to find Geo-Messages</response>
        /// <returns>This returns all Geo-Messages</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// Adds a new Geo-Message
        /// </summary>
        /// <remarks>Authorization is required to create a Geo-Message</remarks>
        /// <param name="geoMessageDto"></param>
        /// <param name="ApiKey"></param>
        /// <response code="201">Geo-Message created</response>
        /// <response code="400">Failed to Create Geo-Message</response>
        /// <response code="401">Failed to authorize</response>
        /// <returns>A newly created Geo-Message</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GeoMessageDto>> CreateGeoMessage([FromQuery] Guid ApiKey, GeoMessageDto geoMessageDto)
        {
            if(geoMessageDto == null)
            {
                return BadRequest();
            }

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
