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

        /// <summary>
        /// Retrieves all Geo-Messages
        /// </summary>
        /// <remarks>These parameters lets you find Geo-Messages by coordinates within a certain area</remarks> 
        /// <param name="minLon">Minimum Longitude</param>
        /// <param name="minLat">Minimum Latitude</param>
        /// <param name="maxLon">Maximum Longitude</param>
        /// <param name="maxLat">Maximum Latitude</param>
        /// <response code="200">Geo-Messages found!</response>
        /// <response code="404">Failed to find Geo-Messages</response>
        /// <returns>This returns all Geo-Messages or by coordinates within a certain area</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            return Ok(geoMessages);
        }


        /// <summary>
        /// Adds a new Geo-Message
        /// </summary>
        /// <remarks>Authorization is required to create a Geo-Message</remarks>
        /// <param name="addGeoMessage"></param>
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
        public async Task<ActionResult<GetGeoMessageDto>> CreateGeoMessage([FromQuery] Guid ApiKey, AddGeoMessageDto addGeoMessage)
        {
            if(addGeoMessage == null)
            {
                return BadRequest();
            }

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
        /// <summary>
        /// Deletes a Geo-Message
        /// </summary>
        /// <remarks>Authorization is required to delete a Geo-Message</remarks>
        /// <param name="ApiKey"></param>
        /// <param name="id"></param>
        /// <response code="200">Geo-Message Deleted</response>
        /// <response code="400">Failed to Delete Geo-Message</response>
        /// <response code="401">Failed to authorize</response>
        /// <returns>A newly deleted Geo-Message</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GeoMessage>> DeleteGeoMessage([FromQuery] Guid ApiKey, int id)
        {
            var geoMessage = await _context.GeoMessages.FirstOrDefaultAsync(g => g.Id == id);

            _context.Remove(geoMessage);
            await _context.SaveChangesAsync();

            return Ok(geoMessage);
        }
    }
}
