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
    public class EmployeesController : Controller
    {
        private static int? OrdinalAsc(int? Ordr, List<Employee> xxx)
        {
            int counter = 0;
            foreach (var item in xxx.OrderBy(i => i.Ordr))
            {
                if (item.Ordr == Ordr)
                {
                    return counter;
                }
                counter++;
            }
            return null;
        }

        private readonly dbcontext _context;
        public async Task<IActionResult> Down(int Ordr)
        {
            var xxx = await _context.Employee.ToListAsync();
            SetOrderIfNull(xxx);
            int? counter = OrdinalAsc(Ordr, xxx);
            if (counter != null)
            {
                if (counter + 1 < xxx.Count())
                {
                    var ss = xxx.OrderBy(i => i.Ordr).ToList()[counter.Value];
                    var tt = xxx.OrderBy(i => i.Ordr).ToList()[counter.Value + 1];

                    int ssid = ss.id;
                    int ssOrdr = ss.Ordr.Value;

                    int ttid = tt.id;
                    int ttOrdr = tt.Ordr.Value;

                    xxx.Where(i => i.id == ss.id).FirstOrDefault().Ordr = ttOrdr;
                    xxx.Where(i => i.id == tt.id).FirstOrDefault().Ordr = ssOrdr;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private static void SetOrderIfNull(List<Employee> xxx)
        {
            foreach (var x in xxx)
            {
                if (x.Ordr == null)
                    x.Ordr = x.id;
            }
        }

        private static int? Ordinal(int? Ordr, List<Employee> xxx)
        {
            int counter = 0;
            foreach (var item in xxx.OrderByDescending(i => i.Ordr))
            {
                if (item.Ordr == Ordr)
                {
                    return counter;
                }
                counter++;
            }
            return null;
        }
        public async Task<IActionResult> Up( int Ordr)//int? id,
        {
            var xxx = await _context.Employee.ToListAsync();
            SetOrderIfNull(xxx);
            int? counter = Ordinal(Ordr, xxx);
            if (counter != null)
            {
                if (counter + 1 < xxx.Count())
                {
                    var ss = xxx.OrderByDescending(i => i.Ordr).ToList()[counter.Value];
                    var tt = xxx.OrderByDescending(i => i.Ordr).ToList()[counter.Value + 1];

                    int ssid = ss.id;
                    int ssOrdr = ss.Ordr.Value;

                    int ttid = tt.id;
                    int ttOrdr = tt.Ordr.Value;

                    xxx.Where(i => i.id == ss.id).FirstOrDefault().Ordr = ttOrdr;
                    xxx.Where(i => i.id == tt.id).FirstOrDefault().Ordr = ssOrdr;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public EmployeesController(dbcontext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
              return View(await _context.Employee.OrderBy(i=>i.Ordr).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Employee employee)
        {
            if (id != employee.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'dbcontext.Employee'  is null.");
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employee.Any(e => e.id == id);
        }
    }
}
