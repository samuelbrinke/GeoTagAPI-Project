using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoTagAPI_Project.Models.V1;
using GeoTagAPI_Project.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeoTagAPI_Project.Dtos;

namespace GeoTagAPI_Project.Controllers.V1
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("1.0")]
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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoMessage>>> GetGeoMessages()
        {
            return await _context.GeoMessages.ToListAsync();
        }


        /// <summary>
        /// Creates a new Geo-Message.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GeoMessage>> CreateGeoMessage(GeoMessage geoMessage)
        {
            var newGeoMessage = new GeoMessage
            {
                Message = geoMessage.Message,
                Latitude = geoMessage.Latitude,
                Longitude = geoMessage.Longitude
            };
            
            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, newGeoMessage);
        }
    }
}
