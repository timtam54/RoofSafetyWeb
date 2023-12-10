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
    public class InspEquipTypeTestFailsController : ControllerBase
    {
        private readonly dbcontext _context;

        public InspEquipTypeTestFailsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/InspEquipTypeTest
        [HttpGet]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<InspEquipTypeTestFail>>> GetInspEquipTypeTestFail(string id)
        {
            if (_context.InspEquipTypeTestFail == null)
            {
                return NotFound();
            }
            return await (from iettf in _context.InspEquipTypeTestFail where iettf.InspEquipTypeTestID == Convert.ToInt32(id) select iettf).ToListAsync();
                //new InspEquipTypeTestRpt { EquipTypeTestID=ett.id, EquipTypeTest=ett.Test, id=iett.id, InspEquipID=iett.InspEquipID, Pass=iett.Pass, Reason=iett.Reason }).ToListAsync();
        }

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<InspEquipTypeTestFail>> GetInspEquipTypeTestFail(int id)
        {
            if (_context.InspEquipTypeTestFail == null)
            {
                return NotFound();
            }
            var inspEquipTypeTestFail = await _context.InspEquipTypeTestFail.FindAsync(id);

            if (inspEquipTypeTestFail == null)
            {
                return NotFound();
            }

            return inspEquipTypeTestFail;
        }

        // PUT: api/InspEquipTypeTest/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInspEquipTypeTestFail(int id, InspEquipTypeTestFail inspEquipTypeTestFail)
        {
            if (id != inspEquipTypeTestFail.id)
            {
                return BadRequest();
            }

            _context.Entry(inspEquipTypeTestFail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InspEquipTypeTestFailExists(id))
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

        // POST: api/InspEquipTypeTest
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InspEquipTypeTest>> PostInspEquipTypeTest(InspEquipTypeTestFail inspEquipTypeTestFail)
        {
            if (_context.InspEquipTypeTestFail == null)
            {
                return Problem("Entity set 'dbContext.InspEquipTypeTestFail'  is null.");
            }
            _context.InspEquipTypeTestFail.Add(inspEquipTypeTestFail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInspEquipTypeTest", new { id = inspEquipTypeTestFail.id }, inspEquipTypeTestFail);
        }

        // DELETE: api/InspEquipTypeTest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInspEquipTypeTestFail(int id)
        {
            if (_context.InspEquipTypeTestFail == null)
            {
                return NotFound();
            }
            var inspEquipTypeTestFail = await _context.InspEquipTypeTestFail.FindAsync(id);
            if (inspEquipTypeTestFail == null)
            {
                return NotFound();
            }

            _context.InspEquipTypeTestFail.Remove(inspEquipTypeTestFail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InspEquipTypeTestFailExists(int id)
        {
            return (_context.InspEquipTypeTestFail?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
