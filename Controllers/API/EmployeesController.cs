using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;

namespace RSSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly dbcontext _context;

        public EmployeesController(dbcontext context)
        {
            _context = context;
        }

        /*     [HttpGet]
             public async Task<ActionResult<IEnumerable<Employee>>> Employees()
             {
                 if (_context.Employee == null)
                 {
                     return NotFound();
                 }
                 return await _context.Employee.ToListAsync();//.Where(i => i.Email == "timhams" && i.Password == "George123")
             }
        */

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee(string id)
        {
            string[] prms = id.Split(Char.Parse("~"), StringSplitOptions.RemoveEmptyEntries);
            string userName = prms[0]; string password= prms[1];
            if (_context.Employee == null)
            {
                return NotFound();
            }
            return await _context.Employee.Where(i => i.Email == userName && i.Password == password).ToListAsync();
        }

      /*  [HttpGet("{userName,password}")]
        public async Task<ActionResult<IEnumerable<Employee>>> LoginUser(string userName,string password)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            return await _context.Employee.Where(i=>i.Email==userName && i.Password==password).ToListAsync();
        }*/

           // GET: api/Employee
           [HttpGet]
           public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
           {
               if (_context.Employee == null)
               {
                   return NotFound();
               }
               return await _context.Employee.OrderBy(i => i.Ordr).ToListAsync();
           }


        [HttpGet("int/{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employee/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employee
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'dbContext.Employee'  is null.");
            }
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.id }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (_context.Employee == null)
            {
                return NotFound();
            }
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employee?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
