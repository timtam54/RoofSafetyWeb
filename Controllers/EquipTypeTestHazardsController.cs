using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Data;
using RoofSafety.Models ;

namespace RoofSafety.Controllers
{
    [Authorize]
    public class EquipTypeTestHazardsController : Controller
    {
        private readonly dbcontext _context;

        public class Etth
        {
            public string Hazard { get;set;  }
            public string EquipType { get; set; }
            
            public string Test { get; set; }

        }

        Etth GetEtth(int hzid, int ettid)
        {
            Etth ret;// = new Etth();
            ret = (from et in _context.EquipType join tst in _context.EquipTypeTest on et.id equals tst.EquipTypeID where tst.id == ettid select new Etth { EquipType= et.EquipTypeDesc , Test= tst.Test }).FirstOrDefault();

            ret.Hazard = _context.Hazard.Find(hzid).Detail;
             return ret;
        }

        public JsonResult Add(int hzid, int ettid)
        {
            Etth eth = GetEtth(hzid, ettid);
            EquipTypeTestHazards etth = _context.EquipTypeTestHazards.Where(i => i.EquipTypeTestID == ettid && i.HazardID == hzid).FirstOrDefault();
            if (etth!=null)
            {
              
                return Json(new { msg = "Hazard `"+eth.Hazard +"` is already present for Equip Type `" + eth.EquipType + "` - Test `" + eth.Test + "`"  });

            }
            etth = new EquipTypeTestHazards();
            etth.HazardID = hzid;
            etth.EquipTypeTestID = ettid;
            _context.Add(etth);
            _context.SaveChanges();

            return Json(new { msg= "Hazard `"+eth.Hazard + "` was successfully added for Equip Type `" + eth.EquipType + "` - Test `" + eth.Test + "`" });
        }

        public JsonResult Del(int hzid, int ettid)
        {
            Etth eth = GetEtth(hzid, ettid);
            EquipTypeTestHazards etth = _context.EquipTypeTestHazards.Where(i=>i.EquipTypeTestID==ettid && i.HazardID==hzid).FirstOrDefault();
            if (etth==null)
                return Json(new { msg = "Hazard `"+eth.Hazard + "` was not found for Equip Type `" + eth.EquipType + "` - Test `" + eth.Test + "`" });

            _context.Remove(etth);
            _context.SaveChanges();
            return Json(new { msg = "Hazard `"+eth.Hazard + "` successfully deleted for Equip Type `" + eth.EquipType + "` - Test `" + eth.Test + "`" });
        }
        public static string Max20(string instring,int mx)
        {
            if (instring == null)
                return "";
            if (instring.Length < mx)
                return instring;
            return instring.Substring(0,mx);
        }
        public EquipTypeTestHazardsController(dbcontext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.id = 0;
            return View(await _context.EquipTypeTestHazards.Include(i=>i.Hazard).Include(i => i.EquipTypeTest).Include(i=>i.EquipTypeTest.EquipType).ToListAsync());
        }

        public async Task<IActionResult> EquipTypeTestHazardMat()
        {
            EquipTypeTestHazardMatrix ret = new EquipTypeTestHazardMatrix();
            ret.Hazards = await _context.Hazard.OrderBy(i => i.Detail).ToListAsync();
            ret.EquipTypeTests = await (from ett in _context.EquipTypeTest join et in _context.EquipType on ett.EquipTypeID equals et.id orderby  et.EquipTypeDesc+ ett.Test select new EquipTypeTestRpt { Test=ett.Test, id=ett.id, EquipTypeID=et.id, EquipTypeName=et.EquipTypeDesc }).ToListAsync();
            ret.EquipTypeTestHazards = await _context.EquipTypeTestHazards.ToListAsync();
            return View(ret);
        }

        public static bool Checked(int HazardID,int EquipTypeTestID,List<EquipTypeTestHazards> ETTHs)
        {
            var fnd = ETTHs.Exists(i => i.HazardID == HazardID && i.EquipTypeTestID == EquipTypeTestID);
            return fnd;
        }

        public async Task<IActionResult> HazardsForEquipTypeTestHazards(int? id)
        {
            try
            {
                ViewBag.id=id;
                var ett = _context.EquipTypeTest.Find(id);

                ViewBag.EquipTypeTest = ett.Test;
                ViewBag.EquipTypeTestID = id;
                ViewBag.EquipTypeID = ett.EquipTypeID;
                @ViewBag.EquipTypeDesc = _context.EquipType.Find(ett.EquipTypeID).EquipTypeDesc;
                var xx = await _context.EquipTypeTestHazards.Where(i => i.EquipTypeTestID == id).Include(i => i.Hazard).ToListAsync();
                return View(xx);
            }
            catch (Exception ex) { 
                var mm = ex; }
            return View();
        }


        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Hazard == null)
        //    {
        //        return NotFound();
        //    }

        //    var hazard = await _context.Hazard
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (hazard == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hazard);
        //}

                public IActionResult Create(int? id)
                {
                    EquipTypeTestHazards ret = new EquipTypeTestHazards();
                    ret.EquipTypeTestID = id.Value;//EquipTypeTestID

                                                   //                    ViewBag.HazardID = (from xx in _context.Hazard select new SelectListItem() { Value = xx.id.ToString(), Text = xx.Detail}).ToList();
            return View(ret);
                }
        //
        [HttpPost]
        //        [ValidateAntiForgeryToken]
        public ActionResult Create(EquipTypeTestHazards equipTypeTestHazards)
        {
            equipTypeTestHazards.id = 0;
            _context.Add(equipTypeTestHazards);
            _context.SaveChanges();
            return RedirectToAction("HazardsForEquipTypeTestHazards", new { id=equipTypeTestHazards.EquipTypeTestID});
        }
            [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
                var xx=_context.EquipTypeTestHazards.Find(id);
            if (xx != null)
            {
                _context.EquipTypeTestHazards.Remove(xx);
                await _context.SaveChangesAsync();
                return Json(new { error = "Success" });
            }
            return Json(new { error = "Record not found" });

        }

        public class IDDesc
        {
            public int ID { get; set; }
            public string Desc { get; set; }
        }

        public JsonResult HazardSearch(string searchString)
        {
            var taxon = (from tx in _context.Hazard
                         where (tx.Detail).Contains(searchString)
                         select tx.id.ToString() + "-" + tx.Detail 
                          ).Take(8).ToList();
            taxon.Add("0-"+searchString + "(add new)");

            return Json(new { results = taxon.OrderBy(i => i) });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Createjs(EquipTypeTestHazards equipTypeTestHazards)
        {
            
//            if (ModelState.IsValid)
            {
                if (equipTypeTestHazards.Hazard!.Detail!.Contains("(add new)"))
                {
                    Hazard hz = new Hazard();
                    hz.Detail = equipTypeTestHazards.Hazard!.Detail.Replace("(add new)", "");
                    _context.Hazard.Add(hz);
                    equipTypeTestHazards.HazardID = hz.id;
                }
                //else
                //{
                //    string[] tuple = equipTypeTestHazards.Haz.Split(new char[] { Char.Parse("-") }, StringSplitOptions.RemoveEmptyEntries);
                //    equipTypeTestHazards.HazardID = Convert.ToInt32(tuple[0]);
                //}
                _context.Add(equipTypeTestHazards);
                try
                {
                    equipTypeTestHazards.id = 0;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
                return Json(new { error = "Success" });
            }
            return Json(new { error = "Not valid" });
        }


        /*        public async Task<IActionResult> Edit(int? id)
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
        */
    }
}
