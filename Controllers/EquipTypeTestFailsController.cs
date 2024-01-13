using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models;

namespace RoofSafety.Controllers
{
    public class EquipTypeTestFailsController : Controller
    {
        private readonly dbcontext _context;

        public EquipTypeTestFailsController(dbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {

            var ett =await _context.EquipTypeTest.FindAsync(id);
            ViewBag.EquipTypeTestDesc = ett?.Test;
            ViewBag.etid = ett?.EquipTypeID;
            var et = await _context.EquipType.FindAsync(ett?.EquipTypeID);
            ViewBag.ettid = id;
            ViewBag.EquipTypeDesc = et?.EquipTypeDesc;
              return View(await _context.EquipTypeTestFail.Where(i=>i.EquipTypeTestID==id).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EquipTypeTestFail == null)
            {
                return NotFound();
            }

            var equipTypeTestFail = await _context.EquipTypeTestFail
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipTypeTestFail == null)
            {
                return NotFound();
            }
            return View(equipTypeTestFail);
        }
        public IActionResult Create(int? id)
        {
            EquipTypeTestFail ret = new EquipTypeTestFail();
            //var equiptype = _context.InspEquip.Where(i => i.id == id).Include(i => i.EquipType).FirstOrDefault();
            ret.EquipTypeTestID = id;
            return View(ret);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipTypeTestFail equipTypeTestFail)
        {
            if (ModelState.IsValid)
            {
                equipTypeTestFail.id = 0;
                _context.Add(equipTypeTestFail);
                await _context.SaveChangesAsync();
                //                return RedirectToAction(nameof(TestsForEquipType),new {id=equipTypeTest.EquipTypeID });
                return RedirectToAction("Index", "EquipTypeTestFails", new { id = equipTypeTestFail.EquipTypeTestID });

            }
            return View(equipTypeTestFail);
        }
        /* 

                public async Task<ActionResult> TestsForEquipType(int id)
                {
                    var xxx = _context.EquipTypeTest.Where(i => i.EquipTypeID == id).Include(i => i.EquipType).Include(i => i.EquipType);
                    var yyy = await xxx.ToListAsync();
                    ViewBag.InspectionID = id;
                    InspectionEquipmentsController.DescParID xx = (from ie in _context.EquipType where ie.id == id select new InspectionEquipmentsController.DescParID { Desc = ("Equipment Type " + ie.EquipTypeDesc), ID = ie.id }).FirstOrDefault();
                    ViewBag.EquipTypeDesc = xx.Desc;
                    ViewBag.EquipTypeID = xx.ID;
                    return View("Index", xxx);
                }


                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Create(EquipTypeTest equipTypeTest,string inspequipid)
                {
                    if (ModelState.IsValid)
                    {
                        equipTypeTest.id = 0;
                        _context.Add(equipTypeTest);
                        await _context.SaveChangesAsync();
                        //                return RedirectToAction(nameof(TestsForEquipType),new {id=equipTypeTest.EquipTypeID });
                        return RedirectToAction(nameof(Create),"InspEquipTypeTestHazards" ,new { id = inspequipid });

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
                }*/

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipTypeTestFail == null)
            {
                return NotFound();
            }

            var equipTypeTestFail = await _context.EquipTypeTestFail
                .FirstOrDefaultAsync(m => m.id == id);
            int? ettid = equipTypeTestFail?.EquipTypeTestID;
            if (ettid==null)
            {
                return NotFound();

            }
            if (equipTypeTestFail == null)
            {
                return NotFound();
            }
            _context.Remove(equipTypeTestFail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index",new {id=ettid });
//            return View(equipTypeTest);
        }


    }
}
