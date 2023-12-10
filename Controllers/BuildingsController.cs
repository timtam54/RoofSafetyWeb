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
    public class BuildingsController : Controller
    {
        private readonly dbcontext _context;

        public BuildingsController(dbcontext context)
        {
            _context = context;
        }

        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            ViewBag.ClientDesc = "All Clients";
            ViewBag.ClientID = 0;

            var dbcontext = _context.Building.Include(b => b.Client);

            return View(await dbcontext.ToListAsync());
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Building == null)
            {
                return NotFound();
            }

            var building = await _context.Building
                .Include(b => b.Client)
                .FirstOrDefaultAsync(m => m.id == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public IActionResult Create(int? id)
        {
            var pp = id;
            Building ret = new Building();
            ret.ClientID = id.Value;
            ViewBag.ClientID = (from xx in _context.Client select  new SelectListItem() { Value=xx.id.ToString(),Text=xx.name }).ToList();
            return View(ret);
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,BuildingName,ClientID,Address")] Building building)
        {
            Client? cli = _context.Client.Find(building.ClientID);
            building.Client = cli!;

           // if (ModelState.IsValid)
            {
                building.id = 0;
                _context.Add(building);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(BuildingsForClient), new {id=building.ClientID });
            }
            ViewBag.ClientID = (from xx in _context.Client select new SelectListItem() { Value = xx.id.ToString(), Text = xx.name }).ToList();

            return View(building);
        }

        public async Task<IActionResult> BuildingsForClient(int? id)
        {
            var dbcontext = _context.Building.Where(b => b.ClientID==id.Value).Include(b => b.Client);
            var res = await dbcontext.ToListAsync();
            ViewBag.ClientDesc = (from ie in _context.Client where ie.id == id select ie.name).FirstOrDefault();
            ViewBag.ClientID = id;
            return View("Index",res);
        }
        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Building == null)
            {
                return NotFound();
            }

            var building = await _context.Building.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            ViewBag.ClientID = (from xx in _context.Client select new SelectListItem() { Value = xx.id.ToString(), Text = xx.name }).ToList();

//            ViewData["ClientID"] = new SelectList(_context.Client, "id", "id", building.ClientID);
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,BuildingName,ClientID,Address")] Building building)
        {
            if (id != building.id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(building);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(BuildingsForClient),new {id=building.ClientID });
            }
            ViewBag.ClientID = (from xx in _context.Client select new SelectListItem() { Value = xx.id.ToString(), Text = xx.name }).ToList();
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Building == null)
            {
                return NotFound();
            }

            var building = await _context.Building
                .Include(b => b.Client)
                .FirstOrDefaultAsync(m => m.id == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Building == null)
            {
                return Problem("Entity set 'dbcontext.Building'  is null.");
            }
            var building = await _context.Building.FindAsync(id);
            if (building != null)
            {
                _context.Building.Remove(building);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingExists(int id)
        {
          return _context.Building.Any(e => e.id == id);
        }
    }
}
