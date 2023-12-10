using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;

namespace RSSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HazardsController : ControllerBase
    {
        private readonly dbcontext _context;

        public HazardsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/Hazard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hazard>>> GetHazard()
        {
            if (_context.Hazard == null)
            {
                return NotFound();
            }
            return await _context.Hazard.OrderBy(i=>i.Detail).ToListAsync();
        }

        // GET: api/Hazard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hazard>> GetHazard(int id)
        {
            if (_context.Hazard == null)
            {
                return NotFound();
            }
            var hazard = await _context.Hazard.FindAsync(id);

            if (hazard == null)
            {
                return NotFound();
            }

            return hazard;
        }

        // PUT: api/Hazard/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHazard(int id, Hazard hazard)
        {
            if (id != hazard.id)
            {
                return BadRequest();
            }

            _context.Entry(hazard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HazardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hazard
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hazard>> PostHazard(Hazard hazard)
        {
            if (_context.Hazard == null)
            {
                return Problem("Entity set 'dbContext.Hazard'  is null.");
            }
            _context.Hazard.Add(hazard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHazard", new { id = hazard.id }, hazard);
        }

        // DELETE: api/Hazard/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHazard(int id)
        {
            if (_context.Hazard == null)
            {
                return NotFound();
            }
            var hazard = await _context.Hazard.FindAsync(id);
            if (hazard == null)
            {
                return NotFound();
            }

            _context.Hazard.Remove(hazard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HazardExists(int id)
        {
            return (_context.Hazard?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
