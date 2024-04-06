using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RoofSafety.Data;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;

namespace RoofSafety.Controllers
{
    [Authorize]
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
            ViewBag.BuildingDesc = "All Buildings";
            List<Inspection> ret = new List<Inspection>();
            if (status == null)
                ret = await _context.Inspection.Where(i => i.Status == "A").OrderByDescending(i => i.InspectionDate).Include(i => i.Building).Include(i => i.Inspector).ToListAsync();
            else
                ret = await _context.Inspection.OrderByDescending(i => i.InspectionDate).Include(i => i.Building).Include(i => i.Inspector).ToListAsync();
            var ss = await (from ie in _context.InspEquip where ret.Select(j => j.id).Contains(ie.InspectionID) group ie by ie.InspectionID into grp select new InspItemCount { InspectionID = grp.Key, Count = grp.Count() }).ToListAsync();
            foreach (var ins in ret)
            {
                var inspit = ss.Where(i => i.InspectionID == ins.id).FirstOrDefault();
                if (inspit != null)
                    ins.Inspector2ID = inspit.Count;
            }
            return View(ret);
        }

        public class InspItemCount
        {
            public int InspectionID { get; set; }
            public int Count { get; set; }
        }

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
            ViewBag.InspectorID= (from xx in _context.Employee orderby xx.Ordr select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();
            Inspection ret = new Inspection();
            ret.BuildingID = id.Value;
            ret.InspectionDate = DateTime.Now.Date;
            return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            inspection.id = 0;
            _context.Add(inspection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InspectionsForBuilding), new { id = inspection.BuildingID });
        }

        public async Task<ActionResult> Copy(int? id)
        {
            int copyid = await CopyInspection(id,-1,_context);
            return RedirectToAction("Edit", new { id = copyid });
        }

        public static async Task<int> CopyInspection(int? id,int InspectorID,dbcontext _context)
        {
            int copyid = 0;
            var existInsp = await _context.Inspection.Where(i => i.id == id).FirstAsync();
            {
                var values = _context.Entry(existInsp).CurrentValues.Clone();
                var newInsp = new Inspection();
                _context.Entry(newInsp).CurrentValues.SetValues(values);
                newInsp.id = 0;
                newInsp.InspectionDate = DateTime.Now;
                if (InspectorID != -1)
                    newInsp.InspectorID = InspectorID;
                newInsp.Status = "A";
                _context.Add(newInsp);
                _context.SaveChanges();

                copyid = newInsp.id;
            }
            var existInspitems = await _context.InspEquip.Where(i => i.InspectionID == id).ToListAsync();
            foreach (var existInspitem in existInspitems)
            {
                var values = _context.Entry(existInspitem).CurrentValues.Clone();
                var newInspEquip = new InspEquip();
                _context.Entry(newInspEquip).CurrentValues.SetValues(values);
                newInspEquip.id = 0;
                newInspEquip.InspectionID = copyid;

                _context.Add(newInspEquip);
                _context.SaveChanges();

                var InspEquipTypeTests = await _context.InspEquipTypeTest.Where(i => i.InspEquipID == existInspitem.id).ToListAsync();
                foreach (var InspEquipTypeTest in InspEquipTypeTests)
                {
                    var values2 = _context.Entry(InspEquipTypeTest).CurrentValues.Clone();
                    var newInspEquipTypeTest = new InspEquipTypeTest();
                    _context.Entry(newInspEquipTypeTest).CurrentValues.SetValues(values2);
                    newInspEquipTypeTest.id = 0;
                    newInspEquipTypeTest.InspEquipID = newInspEquip.id;
                    _context.Add(newInspEquipTypeTest);

                    _context.SaveChanges();

                    var InspEquipTypeTestFails = await _context.InspEquipTypeTestFail.Where(i => i.InspEquipTypeTestID == InspEquipTypeTest.id).ToListAsync();
                    foreach (var InspEquipTypeTestFail in InspEquipTypeTestFails)
                    {
                        var values3 = _context.Entry(InspEquipTypeTestFail).CurrentValues.Clone();
                        var newInspEquipTypeTestFail = new InspEquipTypeTestFail();
                        _context.Entry(newInspEquipTypeTestFail).CurrentValues.SetValues(values3);
                        newInspEquipTypeTestFail.id = 0;
                        newInspEquipTypeTestFail.InspEquipTypeTestID = newInspEquipTypeTest.id;
                        _context.Add(newInspEquipTypeTestFail);

                        _context.SaveChanges();
                    }
                }
            }
            return copyid;
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
            ViewBag.InspectorID = (from xx in _context.Employee orderby xx.Ordr select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();
            ViewBag.BuildingID = (from xx in _context.Building select new SelectListItem() { Value = xx.id.ToString(), Text = xx.BuildingName }).ToList();
            List<SelectListItem> stat = new List<SelectListItem>();
            SelectListItem si;
            si = new SelectListItem(); si.Text = "Active";si.Value = "A";
            stat.Add(si);
            si = new SelectListItem(); si.Text = "Complete"; si.Value = "C";
            stat.Add(si);
            si = new SelectListItem(); si.Text = "Pending"; si.Value = "P";
            stat.Add(si);

            ViewBag.Status = stat;
            return View(inspection);
        }
        [HttpPost]
        public ActionResult Copy()
        {
           return View();
        }


            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
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
                        insp.Inspector2ID = inspection.Inspector2ID;
                        insp.Status= inspection.Status;

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
            //ViewBag.BuildingID = (from xx in _context.Building select new SelectListItem() { Value = xx.id.ToString(), Text = xx.BuildingName }).ToList();
           // ViewBag.InspectorID = (from xx in _context.Employee select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Given + " " + xx.Surname }).ToList();

       //  return View(inspection);
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
