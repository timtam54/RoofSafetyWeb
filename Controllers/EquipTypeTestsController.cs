using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models;

namespace RoofSafety.Controllers
{
    [Authorize]
    public class EquipTypeTestsController : Controller
    {
        private readonly dbcontext _context;

        public EquipTypeTestsController(dbcontext context)
        {
            _context = context;
        }
        public static bool Checked(int etid, string Test, List<EquipTypeTest> ETTs)
        {
            var fnd = ETTs.Exists(i => i.EquipTypeID == etid && i.Test == Test);
            return fnd;
        }
        public async Task<IActionResult> EquipTypeTestMat()
        {
            EquipTypeTestMatrix ret = new EquipTypeTestMatrix();
            ret.EquipTypes = await _context.EquipType.OrderBy(i => i.EquipTypeDesc).ToListAsync();
            ret.EquipTypeTests = await _context.EquipTypeTest.OrderBy(i => i.Test).ToListAsync();//  (from ett in _context.EquipTypeTest join et in _context.EquipType on ett.EquipTypeID equals et.id orderby et.EquipTypeDesc + ett.Test select new EquipTypeTestRpt { Test = ett.Test, id = ett.id, EquipTypeID = et.id, EquipTypeName = et.EquipTypeDesc }).ToListAsync();
                                                                                                 //  ret.EquipTypeTestHazards = await _context.EquipTypeTestHazards.ToListAsync();
            ret.Tests = await _context.EquipTypeTest.GroupBy(i => i.Test).Select(i => i.Key).ToListAsync();
            return View(ret);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }

            var equipTypeTest = await _context.EquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }
            return View(equipTypeTest);
        }

        public IActionResult Create(int? id)
        {
            EquipTypeTest ret = new EquipTypeTest();
            ret.EquipTypeID = id;
            var equiptype = _context.EquipType.Where(i => i.id == id).FirstOrDefault();
            ViewBag.EquipTypeDesc = equiptype?.EquipTypeDesc;
            return View(ret);
        }

        public async Task<IActionResult> Index()
        {

            ViewBag.id = 0;
            EquipTypeTestAll ret = new EquipTypeTestAll();
            ret.ETID = -1;
            ret.ETCopyID = -1;
            ret.Tests = await _context.EquipTypeTest.ToListAsync();
            return View(ret);
        }
     
        public async Task<ActionResult> Copy(int id,int sourceid,string WithHazFail)
        {
            var AddTests = _context.EquipTypeTest.Where(i => i.EquipTypeID == sourceid).ToList();
            var Tests = _context.EquipTypeTest.Where(i => i.EquipTypeID == id).ToList();
            var Hazards = _context.EquipTypeTestHazards.Where(i => AddTests.Select(o => o.id).Contains(i.EquipTypeTestID)).ToList();
            var FailReasons = _context.EquipTypeTestFail.Where(i => AddTests.Select(o => o.id).ToList().Contains(i.EquipTypeTestID!.Value)).ToList();
            foreach (var adtst in AddTests)
            {
                var found = Tests.Where(i => i.Test!.ToLower().Contains(adtst.Test!.ToLower())).ToList();
                if (found.Count==0)
                {
                    EquipTypeTest ett = new EquipTypeTest();
                    ett.EquipTypeID = id;
                    ett.Test = adtst.Test;
                    ett.Severity = adtst.Severity;
                    _context.EquipTypeTest.Add(ett);
                    _context.SaveChanges();
                    if (WithHazFail == "1")
                    {
                        foreach (var adhzrd in Hazards.Where(i => i.EquipTypeTestID == adtst.id))
                        {
                            EquipTypeTestHazards ettHaz = new EquipTypeTestHazards();
                            ettHaz.EquipTypeTestID = ett.id;
                            //ettHaz.Hazard = adhzrd.Hazard;
                            ettHaz.HazardID = adhzrd.HazardID;
                            _context.EquipTypeTestHazards.Add(ettHaz);

                        }
                        foreach (var adflrsn in FailReasons.Where(i => i.EquipTypeTestID == adtst.id))
                        {
                            EquipTypeTestFail ettFR = new EquipTypeTestFail();
                            ettFR.EquipTypeTestID = ett.id;
                            ettFR.FailReason = adflrsn.FailReason;
                            _context.EquipTypeTestFail.Add(ettFR);
                        }
                        _context.SaveChanges();
                    }
                }
            }
           
            return await TFET(id);
        }

        public async Task<ActionResult> TestsForEquipType(int id)
        {
            ViewData["id"] = id;
            
            return await TFET(id);
        }

        private async Task<ActionResult> TFET(int id)
        {
            EquipTypeTestAll ret = new EquipTypeTestAll();
            var xxx = _context.EquipTypeTest.Where(i => i.EquipTypeID == id).Include(i => i.EquipType).Include(i => i.EquipType);
            ret.Tests = await xxx.ToListAsync();
            ret.ETID = id;
            ViewBag.InspectionID = id;
            InspectionEquipmentsController.DescParID xx = (from ie in _context.EquipType where ie.id == id select new InspectionEquipmentsController.DescParID { Desc = ("Equipment Type " + ie.EquipTypeDesc), ID = ie.id }).FirstOrDefault();
            ViewBag.EquipTypeDesc = xx.Desc;
            ViewBag.EquipTypeID = xx.ID;
            List<EquipType> ets = new List<EquipType>();
            EquipType et = new EquipType();
            et.id = 0;
            et.EquipTypeDesc = "-select one to copy from-";
          
            ets.Add(et);
            List<EquipType> etsAdd = await _context.EquipType.OrderBy(i => i.EquipTypeDesc).ToListAsync();
            ets=ets.Concat(etsAdd).ToList();

            ViewBag.ETCopy = ets;
            return View("Index", ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipTypeTest equipTypeTest)//,string inspequipid)
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
            if (ModelState.IsValid)
            {
                equipTypeTest.id = 0;
                _context.Add(equipTypeTest);
                await _context.SaveChangesAsync();
                /////2
                return RedirectToAction("TestsForEquipType", "EquipTypeTests", new {id= equipTypeTest.EquipTypeID });
                //                return RedirectToAction(nameof(TestsForEquipType),new {id=equipTypeTest.EquipTypeID });
                //return RedirectToAction(nameof(Create),"InspEquipTypeTestHazards" ,new { id = inspequipid });

            }
            return View(equipTypeTest);
        }

        public async Task<IActionResult> Edit(int? id,int? ieetid)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }
            ViewBag.iettid = ieetid;//InspEquipID
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);
            ViewBag.EquipTypeTest = equipTypeTest.Test;
            ViewBag.EquipTypeID = equipTypeTest.EquipTypeID;
            if (equipTypeTest == null)
            {
                return NotFound();
            }
            return View(equipTypeTest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( EquipTypeTest equipTypeTest, int iettid)
        {
            ////if (id != equipTypeTest.id)
            ////{
            ////    return NotFound();
            ////}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipTypeTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipTypeTestExists(equipTypeTest.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(TestsForEquipType), new { id = equipTypeTest.EquipTypeID });
                return RedirectToAction(nameof(Edit), "InspEquipTypeTestHazards", new { id = iettid });
            }
            return View(equipTypeTest);
        }

        // GET: EquipTypeTestHazards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipTypeTest == null)
            {
                return NotFound();
            }

            var equipTypeTest = await _context.EquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipTypeTest == null)
            {
                return NotFound();
            }

            return View(equipTypeTest);
        }

        // POST: EquipTypeTestHazards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EquipTypeTest == null)
            {
                return Problem("Entity set 'dbcontext.EquipTypeTest' is null.");
            }
            var equipTypeTest = await _context.EquipTypeTest.FindAsync(id);
            var etid = equipTypeTest.EquipTypeID;
            if (equipTypeTest != null)
            {
                _context.EquipTypeTest.Remove(equipTypeTest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TestsForEquipType), new { id = etid });
        }

        private bool EquipTypeTestExists(int id)
        {
          return _context.EquipTypeTest.Any(e => e.id == id);
        }
    }
}
