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
    public class EquipTypesController : Controller
    {
        private readonly dbcontext _context;

        public EquipTypesController(dbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
              var dd = await _context.EquipType.OrderBy(i => i.EquipTypeDesc).ToListAsync();
            var mm = await _context.EquipTypeTest.Where(i => (dd.Select(j => j.id).ToList().Contains(i.EquipTypeID!.Value))).ToListAsync();
            foreach (var item in dd)
            {
                item.Tests = mm.Where(i => i.EquipTypeID == item.id).Count().ToString() + " test(s)";
/*                foreach (var itm in mm.Where(i => i.EquipTypeID == item.id))
                {
                    if (item.Tests == null || item.Tests == "")
                        item.Tests = itm.Test;
                    else
                        item.Tests = item.Tests + ", " + itm.Test;

                }*/
            }
              return View(dd);

        }

        public async Task<IActionResult> Details(int? id)
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipType equipType)
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
        public async Task<IActionResult> Edit(int id, EquipType equipType)
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
    }
}
