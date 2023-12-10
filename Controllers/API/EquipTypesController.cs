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
    public class EquipTypesController : ControllerBase
    {
        private readonly dbcontext _context;

        public EquipTypesController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/EquipType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipType>>> GetEquipType()
        {
            if (_context.EquipType == null)
            {
                return NotFound();
            }
            return await _context.EquipType.ToListAsync();
        }

        // GET: api/EquipType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EquipType>> GetEquipType(int id)
        {
            if (_context.EquipType == null)
            {
                return NotFound();
            }
            var equipType = await _context.EquipType.FindAsync(id);

            if (equipType == null)
            {
                return NotFound();
            }

            return equipType;
        }

        // PUT: api/EquipType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipType(int id, EquipType equipType)
        {
            if (id != equipType.id)
            {
                return BadRequest();
            }

            _context.Entry(equipType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipTypeExists(id))
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

        // POST: api/EquipType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipType>> PostEquipType(EquipType equipType)
        {
            if (_context.EquipType == null)
            {
                return Problem("Entity set 'dbContext.EquipType'  is null.");
            }
            _context.EquipType.Add(equipType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipType", new { id = equipType.id }, equipType);
        }

        // DELETE: api/EquipType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipType(int id)
        {
            if (_context.EquipType == null)
            {
                return NotFound();
            }
            var equipType = await _context.EquipType.FindAsync(id);
            if (equipType == null)
            {
                return NotFound();
            }

            _context.EquipType.Remove(equipType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipTypeExists(int id)
        {
            return (_context.EquipType?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
