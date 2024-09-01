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
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string bookingType)
        {
            var bookings = from b in _context.Booking
                           select b;

            if (!string.IsNullOrEmpty(bookingType))
            {
                bookings = bookings.Where(b => b.Type == bookingType);
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Date,Time,StudentNo")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check if the slot already exists
                bool slotExists = await _context.Booking
                    .AnyAsync(b => b.Date == booking.Date && b.Time == booking.Time && b.Type == booking.Type);

                if (slotExists)
                {
                    ModelState.AddModelError("", "A slot with the same Date, Time, and Type already exists.");
                    return View(booking);
                }

                // Check if the student already has a booking for the selected date
                var existingBooking = await _context.Booking
                    .Where(b => b.StudentNo == booking.StudentNo && b.Date == booking.Date)
                    .FirstOrDefaultAsync();

                if (existingBooking != null)
                {
                    ModelState.AddModelError("", "The student already has a booking on this date.");
                    return View(booking);
                }

                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Date,Time,StudentNo")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }

        // GET: Bookings/GetTimeSlots
        [HttpGet]
        public async Task<IActionResult> GetTimeSlots(DateTime date, string type)
        {
            int startHour = 8; // 8 AM
            int endHour = 22; // 10 PM

            var timeSlots = new List<TimeSlot>();
            var bookedSlots = await GetBookedSlotsForDateAndType(date, type);

            for (int hour = startHour; hour < endHour; hour += 2)
            {
                var startTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                var endTime = startTime.AddHours(2);
                var startTimeFormatted = startTime.ToString("HH:mm");
                var endTimeFormatted = endTime.ToString("HH:mm");
                var slotDisplay = $"{startTimeFormatted} - {endTimeFormatted}";

                timeSlots.Add(new TimeSlot
                {
                    Value = startTimeFormatted,
                    Display = slotDisplay,
                    IsDisabled = bookedSlots.Contains(startTimeFormatted)
                });
            }

            return Json(new { availableSlots = timeSlots, bookedSlots });
        }

        private async Task<List<string>> GetBookedSlotsForDateAndType(DateTime date, string type)
        {
            var bookedSlots = await _context.Booking
                .Where(b => b.Date.Date == date.Date && b.Type == type)
                .Select(b => b.Time.ToString("HH:mm"))
                .Distinct()
                .ToListAsync();

            return bookedSlots;
        }

        public class TimeSlot
        {
            public string Value { get; set; }
            public string Display { get; set; }
            public bool IsDisabled { get; set; }
        }

    }
}
