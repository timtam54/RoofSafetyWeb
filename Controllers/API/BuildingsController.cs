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
    public class BuildingsController : ControllerBase
    {
        private readonly dbcontext _context;

        public BuildingsController(dbcontext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings(string id)
        {
            if (_context.Building == null)
            {
                return NotFound();
            }
            if (id=="")
                return await _context.Building.ToListAsync();
            var bds = await (from bd in _context.Building join cl in _context.Client on bd.ClientID equals cl.id  where (bd.BuildingName != null && bd.BuildingName.ToLower().Contains(id.ToLower())) || (bd.Address != null && bd.Address.ToLower().Contains(id.ToLower())) || (cl.name!.ToLower().Contains(id.ToLower()))  select new Building { id=bd.id, Address=bd.Address, BuildingName = cl.name +"~"+ bd.BuildingName, ClientID=bd.ClientID } ).ToListAsync();
            return bds;
        }

        // GET: api/Client/5
       
        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<Building>> GetBuilding(int id)
        {
            if (_context.Building == null)
            {
                return NotFound();
            }
            var building = await _context.Building.FindAsync(id);

            if (building == null)
            {
                return NotFound();
            }

            return building;
        }

        // PUT: api/Client/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuilding(int id, Building building)
        {
            if (id != building.id)
            {
                return BadRequest();
            }

            _context.Entry(building).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(id))
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

        // POST: api/Client
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Building>> PostBuilding(Building building)
        {
            if (_context.Building == null)
            {
                return Problem("Entity set 'dbContext.Building'  is null.");
            }
            _context.Building.Add(building);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBuilding", new { id = building.id }, building);
        }

        // DELETE: api/Client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuilding(int id)
        {
            if (_context.Building == null)
            {
                return NotFound();
            }
            var building = await _context.Building.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            _context.Building.Remove(building);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BuildingExists(int id)
        {
            return (_context.Building?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
