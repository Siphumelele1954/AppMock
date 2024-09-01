using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TugwellApp.Data;
using TugwellApp.Models;

namespace TugwellApp.Controllers
{
    public class DCsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DCsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DCs
        public async Task<IActionResult> Index(string searchStudentNo)
        {
            var dCs = from d in _context.DC
                      select d;

            if (!String.IsNullOrEmpty(searchStudentNo))
            {
                dCs = dCs.Where(d => d.StudentNo.Contains(searchStudentNo));
            }

            return View(await dCs.ToListAsync());
        }

        // GET: DCs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dC = await _context.DC
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dC == null)
            {
                return NotFound();
            }

            return View(dC);
        }

        // GET: DCs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DCs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentNo,DateCommitted,Description,PresidingWarden,HearingDate,Iscompleted")] DC dC)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dC);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dC);
        }

        // GET: DCs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dC = await _context.DC.FindAsync(id);
            if (dC == null)
            {
                return NotFound();
            }
            return View(dC);
        }

        // POST: DCs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentNo,DateCommitted,Description,PresidingWarden,HearingDate,Iscompleted")] DC dC)
        {
            if (id != dC.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dC);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DCExists(dC.Id))
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
            return View(dC);
        }

        // GET: DCs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dC = await _context.DC
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dC == null)
            {
                return NotFound();
            }

            return View(dC);
        }

        // POST: DCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dC = await _context.DC.FindAsync(id);
            if (dC != null)
            {
                _context.DC.Remove(dC);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DCExists(int id)
        {
            return _context.DC.Any(e => e.Id == id);
        }
    }
}
