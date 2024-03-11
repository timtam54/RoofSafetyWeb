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
    public class InspEquipTypeTestsController : Controller
    {
        private readonly dbcontext _context;
        public InspEquipTypeTestsController(dbcontext context)
        {
            _context = context;
        }

       class DescID
        {
            public int ID { get; set; } 
            public string Desc { get; set; }
        }
        public async Task<IActionResult> TestsForEquip(int id)
        {
            var xxx = _context.InspEquipTypeTest.Where(i=>i.InspEquipID==id).Include(i=>i.EquipTypeTest);
            var yyy = await xxx.ToListAsync();
            ViewBag.InspEquipID = id;

            var bb = (from ie in _context.InspEquip join et in _context.EquipType on ie.EquipTypeID equals et.id where ie.id == id select new DescID { Desc= et.EquipTypeDesc , ID=ie.InspectionID }).FirstOrDefault();
            ViewBag.EquipDesc = bb.Desc;
            ViewBag.InspectionID = bb.ID;
            return View("Index",yyy);
        }

        public async Task<IActionResult> Index()
        {
            var xxx = _context.InspEquipTypeTest;
            var yyy = await xxx.ToListAsync();
            return View(yyy);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InspEquipTypeTest == null)
            {
                return NotFound();
            }

            var inspEquipTypeTest = await _context.InspEquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspEquipTypeTest == null)
            {
                return NotFound();
            }

            return View(inspEquipTypeTest);
        }

        // GET: InspEquipTypeTestHazards/Create
        public IActionResult Create(int id)//,int 
        {
            InspEquipTypeTest ret = new InspEquipTypeTest();
            int EquipTypeID = (from yy in _context.InspEquip where yy.id == id select yy.EquipTypeID).FirstOrDefault();
            ViewBag.EquipTypeTestID = (from xx in _context.EquipTypeTest where xx.EquipTypeID==EquipTypeID select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Test }).ToList();
            ret.id = 0;
            ret.InspEquipID = id;
            return View(ret);
        }

        // POST: InspEquipTypeTestHazards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspEquipTypeTest inspEquipTypeTest)
        {
            if (ModelState.IsValid)
            {
                inspEquipTypeTest.id = 0;
                _context.Add(inspEquipTypeTest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TestsForEquip),new { id = inspEquipTypeTest.InspEquipID });
            }
            return View(inspEquipTypeTest);
        }

        // GET: InspEquipTypeTestHazards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InspEquipTypeTest == null)
            {
                return NotFound();
            }

            var inspEquipTypeTest = await _context.InspEquipTypeTest.FindAsync(id);
            if (inspEquipTypeTest == null)
            {
                return NotFound();
            }
            InspEquipTypeTestRpt ret = new InspEquipTypeTestRpt();
            ret.EquipTypeTest = inspEquipTypeTest.EquipTypeTest?.Test;
            ret.EquipTypeTestID = inspEquipTypeTest.EquipTypeTestID;
            ret.id = inspEquipTypeTest.id;
            ret.InspEquipID = inspEquipTypeTest.InspEquipID;
            //ret.Pass = inspEquipTypeTest.Pass;
            ret.Comment = inspEquipTypeTest.Comment;
            ret.Reason = (from iettf in _context.InspEquipTypeTestFail join ettf in _context.EquipTypeTestFail on iettf.EquipTypeTestFailID equals ettf.id where iettf.InspEquipTypeTestID == ret.id select ettf.FailReason).ToList();

            int EquipTypeID = _context.InspEquip.Where(i => i.id == inspEquipTypeTest.InspEquipID).Select(i => i.EquipTypeID).FirstOrDefault();

            ViewBag.EquipTypeTestID = (from xx in _context.EquipTypeTest where xx.EquipTypeID == EquipTypeID select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Test }).ToList();
            
            return View(ret);
        }

        // POST: InspEquipTypeTestHazards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,InspEquipTypeTest inspEquipTypeTest)
        {
            if (id != inspEquipTypeTest.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspEquipTypeTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspEquipTypeTestExists(inspEquipTypeTest.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TestsForEquip), new {id= inspEquipTypeTest.InspEquipID });
            }
            return View(inspEquipTypeTest);
        }

        // GET: InspEquipTypeTestHazards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InspEquipTypeTest == null)
            {
                return NotFound();
            }

            var inspEquipTypeTest = await _context.InspEquipTypeTest
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspEquipTypeTest == null)
            {
                return NotFound();
            }

            return View(inspEquipTypeTest);
        }

        // POST: InspEquipTypeTestHazards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InspEquipTypeTest == null)
            {
                return Problem("Entity set 'dbcontext.InspEquipTypeTest'  is null.");
            }
            var inspEquipTypeTest = await _context.InspEquipTypeTest.FindAsync(id);
            if (inspEquipTypeTest != null)
            {
                _context.InspEquipTypeTest.Remove(inspEquipTypeTest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspEquipTypeTestExists(int id)
        {
          return _context.InspEquipTypeTest.Any(e => e.id == id);
        }
    }
}
