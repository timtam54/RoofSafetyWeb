using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;
using Version = RoofSafety.Models.Version;
using System.Runtime.Intrinsics.X86;

namespace RSSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionsController : ControllerBase
    {
        private readonly dbcontext _context;

        public VersionsController(dbcontext context)
        {
            _context = context;
        }


        [HttpGet("{id}")]

        public async Task<ActionResult<IEnumerable<VersionRpt>>> GetVersion(string id)
        {
            //if (_context.Version == null)
            //{
            //    return NotFound();
            //}
            //var bid = await _context.Inspection.FindAsync(Convert.ToInt32( id));
            int buildingid = Convert.ToInt32(id);// bid.BuildingID;
            var ret= await (from vs in _context.Inspection join emp in _context.Employee on vs.InspectorID equals emp.id where vs.BuildingID == buildingid orderby vs.id select new VersionRpt { Photo=vs.Photo, Areas = vs.Areas, TestingInstruments = vs.TestingInstruments ,id = vs.id, Information = vs.InspectionDate.ToString("dd-MM-yyyy"), Author = emp.Given + " " + emp.Surname, VersionNo = vs.id, VersionType = (vs.Status == "A") ? "Active" : (vs.Status == "P") ? "Pending" : "Complete" }).ToListAsync();
            int vn = 1;
            foreach (var rr in ret)
            {
                rr.VersionNo = vn++;
            }
            return ret;
            //return await (from vs in _context.Version join emp in _context.Employee on vs.AuthorID equals emp.id where vs.InspectionID==Convert.ToInt32(id) select new VersionRpt { id=vs.id, Information=vs.Information, Author=emp.Given + " " + emp.Surname, VersionNo=vs.VersionNo, VersionType=(vs.VersionType=="FD")?"First Draft": "Internal Review"} ).ToListAsync();
        }
     

        // GET: api/Version/5
        //[HttpGet("int/{id:int}")]
        //public async Task<ActionResult<Version>> GetVersion(int id)
        //{
        //    if (_context.Version == null)
        //    {
        //        return NotFound();
        //    }
        //    return (await _context.Version.FindAsync( Convert.ToInt32(id)))!;
        //}

        // PUT: api/Version/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       // [HttpPut("{id}")]
        //public async Task<IActionResult> PutVersion(int id, Version version)
        //{
        //    if (id != version.id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(version).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!VersionExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Version
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Version>> PostVersion(Version version)
        //{
        //    if (_context.Version == null)
        //    {
        //        return Problem("Entity set 'dbContext.Version'  is null.");
        //    }
        //    _context.Version.Add(version);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetEquipType", new { id = version.id }, version);
        //}

        // DELETE: api/Version/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteVersion(int id)
        //{
        //    if (_context.Version == null)
        //    {
        //        return NotFound();
        //    }
        //    var version = await _context.Version.FindAsync(id);
        //    if (version == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Version.Remove(version);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool VersionExists(int id)
        //{
        //    return (_context.Version?.Any(e => e.id == id)).GetValueOrDefault();
        //}
    }
}
