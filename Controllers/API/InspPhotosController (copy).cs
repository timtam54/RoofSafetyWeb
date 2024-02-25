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
    public class InspPhotosController : ControllerBase
    {
        private readonly dbcontext _context;

        public InspPhotosController(dbcontext context)
        {
            _context = context;
        }

        // GET: api/InspPhoto
        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<InspPhoto>>> GetInspPhoto(string id)
        {
            string[] val= id.Split('~');
            if (_context.InspPhoto == null)
            {
                return NotFound();
            }
            return await _context.InspPhoto.Where(i => i.InspEquipID == Convert.ToInt32(val[0]) && i.SourceTable == val[1]).ToListAsync();
        }

       

     

        // GET: api/InspPhoto/5
        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<IEnumerable<InspPhoto>>> GetInspPhoto(int id)
        {
            //string[] xx = id.Split('~');
            if (_context.InspPhoto == null)
            {
                return NotFound();
            }
            return await _context.InspPhoto.Where(i => i.id == Convert.ToInt32(id)).ToListAsync();
        }

        // PUT: api/InspPhoto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInspPhoto(int id, InspPhoto inspPhoto)
        {
            if (id != inspPhoto.id)
            {
                return BadRequest();
            }

            _context.Entry(inspPhoto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InspPhotoExists(id))
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

        // POST: api/InspPhoto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InspPhoto>> PostInspPhoto(InspPhoto inspPhoto)
        {
            if (_context.InspPhoto == null)
            {
                return Problem("Entity set 'dbContext.InspPhoto'  is null.");
            }
            _context.InspPhoto.Add(inspPhoto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipType", new { id = inspPhoto.id }, inspPhoto);
        }

        // DELETE: api/InspPhoto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInspPhoto(int id)
        {

            if (_context.InspPhoto == null)
            {
                return NotFound();
            }
            var inspPhoto = await _context.InspPhoto.FindAsync(id);
            if (inspPhoto == null)
            {
                return NotFound();
            }

            _context.InspPhoto.Remove(inspPhoto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InspPhotoExists(int id)
        {
            return (_context.InspPhoto?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
