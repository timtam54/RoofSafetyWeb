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
    public class InspEquipTypeTestsController : ControllerBase
    {
        private readonly dbcontext _context;

        public InspEquipTypeTestsController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/InspEquipTypeTest
        [HttpGet]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<InspEquipTypeTestRpt>>> GetInspEquipTypeTest(string id)
        {
            if (_context.InspEquipTypeTest == null)
            {
                return NotFound();
            }
            return await (from iett in _context.InspEquipTypeTest join ett in _context.EquipTypeTest on iett.EquipTypeTestID equals ett.id where iett.InspEquipID==Convert.ToInt32(id) select new InspEquipTypeTestRpt { Severity= ett.Severity, Comment=iett.Comment, EquipTypeTestID=ett.id, EquipTypeTest=ett.Test, id=iett.id, InspEquipID=iett.InspEquipID/*, Pass=iett.Pass*/ }).ToListAsync();
        }

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<InspEquipTypeTest>> GetInspEquipTypeTest(int id)
        {
            if (_context.InspEquipTypeTest == null)
            {
                return NotFound();
            }
            var inspEquipTypeTest = await _context.InspEquipTypeTest.FindAsync(id);

            if (inspEquipTypeTest == null)
            {
                return NotFound();
            }

            return inspEquipTypeTest;
        }

        // PUT: api/InspEquipTypeTest/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInspEquipTypeTest(int id, InspEquipTypeTest inspEquipTypeTest)
        {
            if (id != inspEquipTypeTest.id)
            {
                return BadRequest();
            }

            _context.Entry(inspEquipTypeTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InspEquipTypeTestExists(id))
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
        public async Task<ActionResult<InspEquipTypeTest>> PostInspEquipTypeTest(InspEquipTypeTest inspEquipTypeTest)
        {
            if (_context.InspEquipTypeTest == null)
            {
                return Problem("Entity set 'dbContext.InspEquipTypeTest'  is null.");
            }
            _context.InspEquipTypeTest.Add(inspEquipTypeTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInspEquipTypeTest", new { id = inspEquipTypeTest.id }, inspEquipTypeTest);
        }

        // DELETE: api/InspEquipTypeTest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInspEquipTypeTest(int id)
        {
            if (_context.InspEquipTypeTest == null)
            {
                return NotFound();
            }
            var inspEquipTypeTest = await _context.InspEquipTypeTest.FindAsync(id);
            if (inspEquipTypeTest == null)
            {
                return NotFound();
            }

            _context.InspEquipTypeTest.Remove(inspEquipTypeTest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InspEquipTypeTestExists(int id)
        {
            return (_context.InspEquipTypeTest?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
