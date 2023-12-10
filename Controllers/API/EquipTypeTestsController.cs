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
    public class EquipTypeTestsController : ControllerBase
    {
        private readonly dbcontext _context;

        public EquipTypeTestsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/EquipTypeTest
    //    [HttpGet]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EquipTypeTest>>> GetEquipType(string id)
        {
            if (_context.EquipTypeTest == null)
            {
                return NotFound();
            }
            return await _context.EquipTypeTest.Where(i=>i.EquipTypeID==Convert.ToInt32(id)).ToListAsync();
        }

        // GET: api/EquipTypeTest/5

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<EquipTypeTest>> GetEquipType(int id)
        {
            if (_context.EquipTypeTest == null)
            {
                return NotFound();
            }
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);

            if (equipTypeTest == null)
            {
                return NotFound();
            }

            return equipTypeTest;
        }

        // PUT: api/EquipTypeTest/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipType(int id, EquipTypeTest equipTypeTest)
        {
            if (id != equipTypeTest.id)
            {
                return BadRequest();
            }

            _context.Entry(equipTypeTest).State = EntityState.Modified;

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

        // POST: api/EquipTypeTest
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EquipTypeTest>> PostEquipType(EquipTypeTest equipTypeTest)
        {
            if (_context.EquipTypeTest == null)
            {
                return Problem("Entity set 'dbContext.EquipTypeTest'  is null.");
            }
            _context.EquipTypeTest.Add(equipTypeTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipType", new { id = equipTypeTest.id }, equipTypeTest);
        }

        // DELETE: api/EquipTypeTest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipType(int id)
        {
            if (_context.EquipTypeTest == null)
            {
                return NotFound();
            }
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }
            if (_context.InspEquipTypeTest.Where(i=>i.EquipTypeTestID==id).Count()>0)
            {
                return BadRequest("record referecned b yinspection test - cannot delete");
            }
            _context.EquipTypeTest.Remove(equipTypeTest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipTypeExists(int id)
        {
            return (_context.EquipTypeTest?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
