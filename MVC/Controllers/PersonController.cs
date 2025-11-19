using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using ClosedXML.Excel;

namespace MVC.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Person
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persons.ToListAsync());
        }

        // ---------------- Upload Excel (ClosedXML) ----------------
        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file để upload!";
                return RedirectToAction(nameof(Index));
            }

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".xlsx" && ext != ".xls")
            {
                TempData["Error"] = "Chỉ chấp nhận file Excel (.xlsx, .xls)";
                return RedirectToAction(nameof(Index));
            }

            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "ExcelsFiles");
            Directory.CreateDirectory(uploads);

            string filePath = Path.Combine(uploads, $"{Guid.NewGuid()}{ext}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            int count = 0;

            using (var workbook = new XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1);
                var rows = ws.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1)) // bỏ header
                {
                    string personId = row.Cell(1).GetString().Trim();
                    if (string.IsNullOrEmpty(personId)) continue;

                    if (!_context.Persons.Any(p => p.PersonId == personId))
                    {
                        var person = new Person
                        {
                            PersonId = personId,
                            FullName = row.Cell(2).GetString(),
                            Address = row.Cell(3).GetString(),
                            Email = row.Cell(4).GetString()
                        };

                        await _context.Persons.AddAsync(person);
                        count++;
                    }
                }

                await _context.SaveChangesAsync();
            }

            TempData["Message"] = $"Đã import {count} dòng từ file Excel.";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- Download Excel (ClosedXML) ----------------
        public IActionResult Download()
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Persons");

                // Header
                ws.Cell(1, 1).Value = "PersonId";
                ws.Cell(1, 2).Value = "FullName";
                ws.Cell(1, 3).Value = "Address";
                ws.Cell(1, 4).Value = "Email";

                // Data
                var list = _context.Persons.ToList();
                int row = 2;

                foreach (var p in list)
                {
                    ws.Cell(row, 1).Value = p.PersonId;
                    ws.Cell(row, 2).Value = p.FullName;
                    ws.Cell(row, 3).Value = p.Address;
                    ws.Cell(row, 4).Value = p.Email;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Persons.xlsx");
                }
            }
        }

        // ---------------- CRUD ----------------
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == id);
            if (person == null) return NotFound();
            return View(person);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,Address,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonId,FullName,Address,Email")] Person person)
        {
            if (id != person.PersonId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Persons.Any(e => e.PersonId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var person = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
