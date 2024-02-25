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
    public class inspEquipsController : ControllerBase
    {

       


        private readonly dbcontext _context;

        public inspEquipsController(dbcontext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<InspEquipView>>> GetinspEquips(string id)
        {
            if (_context.InspEquip == null)
            {
                return NotFound();
            }
            var yyy = from ie in _context.InspEquip join et in _context.EquipType on ie.EquipTypeID equals et.id orderby ie.id descending where ie.InspectionID==Convert.ToInt32(id) select new InspEquipView { id = ie.id, InspectionID = ie.InspectionID, Installer = ie.Installer, Location = ie.Location, EquipDesc = et.EquipTypeDesc, Manufacturer = ie.Manufacturer, WithdrawalDate = ie.WithdrawalDate, SerialNo = ie.SerialNo,  Rating = ie.Rating, Qty=ie.Qty };
            var zzz = await (yyy).ToListAsync();
            foreach (var item in zzz)
            {
                var ph = _context.InspPhoto.Where(i => i.InspEquipID == item.id).FirstOrDefault();
                if (ph != null)
                    item.Photos = ph.photoname;
            }

            return zzz;
        }

        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<InspEquip>> GetinspEquip(int id)
        {
            if (_context.InspEquip == null)
            {
                return NotFound();
            }
            var inspectionEquipment = await _context.InspEquip.FindAsync(id);

            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            return inspectionEquipment;
        }

        // PUT: api/inspEquip/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutinspEquip(int id, InspEquip inspectionEquipment)
        {
            if (id != inspectionEquipment.id)
            {
                return BadRequest();
            }

            _context.Entry(inspectionEquipment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!inspEquipExists(id))
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

        // POST: api/inspEquip
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InspEquip>> PostinspEquip(InspEquip inspectionEquipment)
        {
            if (_context.InspEquip == null)
            {
                return Problem("Entity set 'dbContext.inspEquip'  is null.");
            }
            _context.InspEquip.Add(inspectionEquipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetinspEquip", new { id = inspectionEquipment.id }, inspectionEquipment);
        }

        // DELETE: api/inspEquip/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteinspEquip(int id)
        {
            if (_context.InspEquip == null)
            {
                return NotFound();
            }
            var inspectionEquipment = await _context.InspEquip.FindAsync(id);
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            _context.InspEquip.Remove(inspectionEquipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool inspEquipExists(int id)
        {
            return (_context.InspEquip?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
