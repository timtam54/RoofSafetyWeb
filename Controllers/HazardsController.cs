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
    public class HazardsController : Controller
    {
        private readonly dbcontext _context;
        public HazardsController(dbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hazard == null)
            {
                return NotFound();
            }

            var hz = await _context.Hazard.FindAsync(id);
            if (hz == null)
            {
                return NotFound();
            }
            return View(hz);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hazard == null)
            {
                return NotFound();
            }
            var etth = await _context.EquipTypeTestHazards.CountAsync(i => i.HazardID == id);
            if (etth!=0)
            {
                var hzx = await _context.Hazard.Where(i => i.id == id).FirstOrDefaultAsync();
                return RedirectToAction("Index",new { Error="there are "+etth.ToString()+" equipment type tests associated with this hazard ("+ hzx.Detail + ")- cannot be deleted"});
            }
            var hz = await _context.Hazard.FindAsync(id);
       
            if (hz == null)
            {
                return NotFound();
            }
            _context.Hazard.Remove(hz);
            _context.SaveChanges();
            return RedirectToAction("Index", new { Error = "Hazard Deleted :)" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Detail")] Hazard hazard)
        {
            if (id != hazard.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hazard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!EquipTypeExists(equipType.id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hazard);
        }
        public async Task<IActionResult> Index(string Error)
        {
            ViewBag.Error = Error;
              return View(await _context.Hazard.OrderBy(i=>i.Detail).ToListAsync());
        }

        public JsonResult Add(string text)
        {
            Hazard hz = new Hazard();
            hz.Detail = text.Replace("(add new)","");
            _context.Add(hz);
            _context.SaveChanges();
            return Json(new { num = hz.id });
        }

        public JsonResult Get(int id)
        {
            var taxon = (from tx in _context.Hazard
                         where tx.id == id
                         select tx.Detail
                          ).FirstOrDefault();

            return Json(new { results = taxon });
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hazard == null)
            {
                return NotFound();
            }

            var hazard = await _context.Hazard
                .FirstOrDefaultAsync(m => m.id == id);
            if (hazard == null)
            {
                return NotFound();
            }

            return View(hazard);
        }
/*        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,EquipTypeDesc")] EquipType equipType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipType);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EquipType == null)
            {
                return NotFound();
            }

            var equipType = await _context.EquipType.FindAsync(id);
            if (equipType == null)
            {
                return NotFound();
            }
            return View(equipType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,EquipTypeDesc")] EquipType equipType)
        {
            if (id != equipType.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipTypeExists(equipType.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(equipType);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EquipType == null)
            {
                return NotFound();
            }

            var equipType = await _context.EquipType
                .FirstOrDefaultAsync(m => m.id == id);
            if (equipType == null)
            {
                return NotFound();
            }

            return View(equipType);
        }

        // POST: EquipTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EquipType == null)
            {
                return Problem("Entity set 'dbcontext.EquipType'  is null.");
            }
            var equipType = await _context.EquipType.FindAsync(id);
            if (equipType != null)
            {
                _context.EquipType.Remove(equipType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipTypeExists(int id)
        {
          return _context.EquipType.Any(e => e.id == id);
        }
*/
    }
}
