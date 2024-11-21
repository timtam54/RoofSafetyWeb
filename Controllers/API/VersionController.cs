
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;


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

            int buildingid = Convert.ToInt32(id);// bid.BuildingID;
            int NextInspMonths = 12;
            var ret = await (from vs in _context.Inspection join emp in _context.Employee on vs.InspectorID equals emp.id where vs.BuildingID == buildingid orderby vs.id select new VersionRpt { Author2 = (vs.Inspector2ID == null) ? null : vs.Inspector2ID.ToString(), Photo = vs.Photo, Areas = vs.Areas, TestingInstruments = vs.TestingInstruments, id = vs.id, NextDue= vs.InspectionDate.ToString("dd-MM-yyyy"),Information = vs.InspectionDate.ToString("dd-MM-yyyy"), Author = emp.Given + " " + emp.Surname, VersionNo = vs.id, VersionType = (vs.Status == "A") ? "Active" : (vs.Status == "P") ? "Pending" : "Complete" }).ToListAsync();
            int vn = 1;
            foreach (var rr in ret)
            {
                rr.VersionNo = vn++;
                var insp2 = _context.Employee.Where(i => i.id.ToString() == rr.Author2).FirstOrDefault();
                if (insp2 != null)
                    rr.Author2 = insp2.Given + " " + insp2.Surname;
            }
            return ret;
        }      
    }
}
