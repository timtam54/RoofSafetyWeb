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
    public class EquipTypeTestFailsController : ControllerBase
    {
        private readonly dbcontext _context;

        public EquipTypeTestFailsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/EquipTypeTestFail
    //    [HttpGet]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EquipTypeTestFail>>> GetEquipTypeTest(string id)
        {
            if (_context.EquipTypeTestFail == null)
            {
                return NotFound();
            }
            return await _context.EquipTypeTestFail.Where(i=>i.EquipTypeTestID==Convert.ToInt32(id)).ToListAsync();
        }

        // GET: api/EquipTypeTestFail/5

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<EquipTypeTestFail>> GetEquipTypeTest(int id)
        {
            if (_context.EquipTypeTestFail == null)
            {
                return NotFound();
            }
            var equipTypeTest = await _context.EquipTypeTestFail.FindAsync(id);

            if (equipTypeTest == null)
            {
                return NotFound();
            }

            return equipTypeTest;
        }

        // PUT: api/EquipTypeTestFail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipTypeTestFail(int id, EquipTypeTestFail equipTypeTestFail)
        {
            if (id != equipTypeTestFail.id)
            {
                return BadRequest();
            }

            _context.Entry(equipTypeTestFail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipTypeFailExists(id))
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

        // POST: api/EquipTypeTestFail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipTypeTestFail>> PostEquipTypeTest(EquipTypeTestFail equipTypeTestFail)
        {
            if (_context.EquipTypeTestFail == null)
            {
                return Problem("Entity set 'dbContext.EquipTypeTestFail'  is null.");
            }
            _context.EquipTypeTestFail.Add(equipTypeTestFail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipType", new { id = equipTypeTestFail.id }, equipTypeTestFail);
        }

        // DELETE: api/EquipTypeTestFail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipTypeTestFail(int id)
        {
            if (_context.EquipTypeTestFail == null)
            {
                return NotFound();
            }
            var equipTypeTest = await _context.EquipTypeTestFail.FindAsync(id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }
            if (_context.InspEquipTypeTest.Where(i=>i.EquipTypeTestID==id).Count()>0)
            {
                return BadRequest("record referecned b yinspection test - cannot delete");
            }
            _context.EquipTypeTestFail.Remove(equipTypeTest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipTypeFailExists(int id)
        {
            return (_context.EquipTypeTestFail?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
