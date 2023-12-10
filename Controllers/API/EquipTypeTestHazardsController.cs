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
    public class EquipTypeTestHazardsController : ControllerBase
    {
        private readonly dbcontext _context;

        public EquipTypeTestHazardsController(dbcontext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EquipTypeTestHazards>>> GetEquipTypeTestHazard(string id)
        {
            if (_context.EquipTypeTestHazards == null)
            {
                return NotFound();
            }
            return await _context.EquipTypeTestHazards.Where(i=>i.EquipTypeTestID==Convert.ToInt32(id)).ToListAsync();
        }

        // GET: api/EquipTypeTestHazards/5

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<EquipTypeTestHazards>> GetEquipTypeTestHazard(int id)
        {
            if (_context.EquipTypeTestHazards == null)
            {
                return NotFound();
            }
            var equipTypeTestHazard = await _context.EquipTypeTestHazards.FindAsync(id);

            if (equipTypeTestHazard == null)
            {
                return NotFound();
            }

            return equipTypeTestHazard;
        }

        // PUT: api/EquipTypeTestHazards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipTypeTestHazard(int id, EquipTypeTestHazards equipTypeTestHazard)
        {
            if (id != equipTypeTestHazard.id)
            {
                return BadRequest();
            }

            _context.Entry(equipTypeTestHazard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipTypeTestHazardExists(id))
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

        // POST: api/EquipTypeTestHazards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipTypeTestHazards>> PostEquipTypeTestHazard(EquipTypeTestHazards equipTypeTestHazard)
        {
            if (_context.EquipTypeTestHazards == null)
            {
                return Problem("Entity set 'dbContext.EquipTypeTestHazards'  is null.");
            }
            _context.EquipTypeTestHazards.Add(equipTypeTestHazard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipType", new { id = equipTypeTestHazard.id }, equipTypeTestHazard);
        }

        // DELETE: api/EquipTypeTestHazards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipTypeTestHazard(int id)
        {
            if (_context.EquipTypeTestHazards == null)
            {
                return NotFound();
            }
            var equipTypeTestHazard = await _context.EquipTypeTestHazards.FindAsync(id);
            if (equipTypeTestHazard == null)
            {
                return NotFound();
            }
          //  if (_context.EquipTypeTest.Where(i=>i.EquipTypeTestID==id).Count()>0)
            //{
              //  return BadRequest("record referecned b yinspection test - cannot delete");
            //}
            _context.EquipTypeTestHazards.Remove(equipTypeTestHazard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipTypeTestHazardExists(int id)
        {
            return (_context.EquipTypeTestHazards?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
