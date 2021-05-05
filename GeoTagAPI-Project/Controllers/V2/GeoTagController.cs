using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoTagAPI_Project.Models.V2;
using GeoTagAPI_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GeoTagAPI_Project.Controllers.V2
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("2.0")]
    public class GeoTagController : ControllerBase
    {
        private readonly GeoTagDbContext _context;
        public GeoTagController(GeoTagDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GeoMessage>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.FindAsync(id);

            if (geoMessage == null)
                return NotFound();

            return Ok(geoMessage);
        }

        /*[HttpGet("{minLon, minLat, maxLon, maxLat}")]
        public async Task<ActionResult<IEnumerable<GeoMessage>>> GetGeoMessages(double minLon, double minLat, double maxLon, double maxLat)
        {
            return await _context.GeoMessages.ToListAsync();
        }
        */

        /// <summary>
        /// Creates a new Geo-Message.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<string>> CreateGeoMessage(GeoMessage geoMessage)
        {

            /*
            var newGeoMessage = new GeoMessage
            {
                Message = geoMessage.Message,
                Latitude = geoMessage.Latitude,
                Longitude = geoMessage.Longitude
            };
            */
            //await _context.AddAsync(newGeoMessage);
            //await _context.SaveChangesAsync();
            
            return Ok();
            //return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, newGeoMessage);
        }
    }
}
