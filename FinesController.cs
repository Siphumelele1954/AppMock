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
    public class FinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fines
        public async Task<IActionResult> Index(string searchStudentNo)
        {
            var fines = from f in _context.Fine
                        select f;

            if (!String.IsNullOrEmpty(searchStudentNo))
            {
                fines = fines.Where(s => s.StudentNo.Contains(searchStudentNo));
            }

            return View(await fines.ToListAsync());
        }


        // GET: Fines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fine
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // GET: Fines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateCommitted,Description,StudentNo,FineAmount,IsPaid")] Fine fine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fine);
        }

        // GET: Fines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fine.FindAsync(id);
            if (fine == null)
            {
                return NotFound();
            }
            return View(fine);
        }

        // POST: Fines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateCommitted,Description,StudentNo,FineAmount,IsPaid")] Fine fine)
        {
            if (id != fine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FineExists(fine.Id))
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
            return View(fine);
        }

        // GET: Fines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fine = await _context.Fine
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fine == null)
            {
                return NotFound();
            }

            return View(fine);
        }

        // POST: Fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fine = await _context.Fine.FindAsync(id);
            if (fine != null)
            {
                _context.Fine.Remove(fine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FineExists(int id)
        {
            return _context.Fine.Any(e => e.Id == id);
        }
    }
}
