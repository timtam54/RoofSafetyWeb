using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;

namespace RoofSafety.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly dbcontext _context;
        private readonly IImageService _imageservice;

        public InspectionsController(dbcontext context, IImageService imageservice)
        {
            _context = context;
            _imageservice = imageservice;
        }

        public async Task<IActionResult> Index(string? status)
        {
            ViewBag.ClientDesc = "All Clients";
            ViewBag.ClientID = 0;

            ViewBag.BuildingID = 0;
           // var bd = (from ie in _context.Building where ie.id == id select ie).FirstOrDefault();
            ViewBag.BuildingDesc = "All Buildings";
            if (status==null)
                return View(await _context.Inspection.Where(i=>i.Status=="A").Include(i => i.Building).Include(i => i.Inspector).ToListAsync());
            return View(await _context.Inspection.Include(i => i.Building).Include(i=>i.Inspector).ToListAsync());
        }

        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Inspection == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspection
                .Include(i => i.Building)
                .Include(i => i.Inspector)
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspection == null)
            {
                return NotFound();
            }
            return View(inspection);
        }

        public IActionResult Create(int? id)
        {
            ViewBag.BuildingID = (from xx in _context.Building select new SelectListItem() { Value = xx.id.ToString(), Text = xx.BuildingName }).ToList();
            ViewBag.InspectorID= (from xx in _context.Employee select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();
            Inspection ret = new Inspection();
            ret.BuildingID = id.Value;
            ret.InspectionDate = DateTime.Now.Date;
            return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,InspectionDate,Areas,BuildingID,InspectorID,TestingInstruments")] Inspection inspection)
        {
      //      if (ModelState.IsValid)
            {
                inspection.id = 0;  
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(InspectionsForBuilding), new {id= inspection.BuildingID });
            }
//            ViewData["BuildingID"] = new SelectList(_context.Building, "id", "id", inspection.BuildingID);
  //          return View(inspection);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Inspection == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspection.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }
            if (inspection.Photo!=null)
                inspection.Photo = _imageservice.GetImageURL(inspection.Photo);
            ViewBag.InspectorID = (from xx in _context.Employee select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();
            ViewBag.BuildingID = (from xx in _context.Building select new SelectListItem() { Value = xx.id.ToString(), Text = xx.BuildingName }).ToList();
            return View(inspection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,InspectionDate,Areas,BuildingID,InspectorID,TestingInstruments")] Inspection inspection)
        {
            if (id != inspection.id)
            {
                return NotFound();
            }

//            if (ModelState.IsValid)
            {
                try
                {
                    var insp = _context.Inspection.Find(id);
                    if (insp != null)
                    {
                        insp.InspectionDate = inspection.InspectionDate;
                        insp.Areas = inspection.Areas;
                        insp.BuildingID = inspection.BuildingID;
                        insp.InspectorID = inspection.InspectorID;
                        insp.TestingInstruments = inspection.TestingInstruments;
                        //inspection.id = 0;
                        _context.Update(insp);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionExists(inspection.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(InspectionsForBuilding),new { id=inspection.BuildingID});
            }
            ViewBag.BuildingID = (from xx in _context.Building select new SelectListItem() { Value = xx.id.ToString(), Text = xx.BuildingName }).ToList();
            ViewBag.InspectorID = (from xx in _context.Employee select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();

            return View(inspection);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Inspection == null)
            {
                return NotFound();
            }
            var inspection = await _context.Inspection
                .Include(i => i.Building)
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspection == null)
            {
                return NotFound();
            }
            return View(inspection);
        }
        public async Task<IActionResult> InspectionsForBuilding(int id)
        {
            var dbcontext = _context.Inspection.Where(i => i.BuildingID==id).Include(i=>i.Building).Include(i=>i.Inspector);
            var res = await dbcontext.ToListAsync();

            ViewBag.BuildingID = id;
            var bd = (from ie in _context.Building where ie.id == id select ie).FirstOrDefault();
            if (bd != null)
            {
                ViewBag.BuildingDesc = bd.BuildingName;
                ViewBag.ClientID = bd.ClientID;
            }
            return View("Index",res);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Inspection == null)
            {
                return Problem("Entity set 'dbcontext.Inspection'  is null.");
            }
            var inspection = await _context.Inspection.FindAsync(id);
            int bid = inspection.BuildingID;
            if (inspection != null)
            {
                _context.Inspection.Remove(inspection);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InspectionsForBuilding), new { id = bid });

           // return RedirectToAction(nameof(Index));
        }

        private bool InspectionExists(int id)
        {
          return _context.Inspection.Any(e => e.id == id);
        }
    }
}
