using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using System.Data;
using System.IO;

namespace MVC.Controllers
{
    public class PersonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelProcess _excelProcess = new ExcelProcess();

        public PersonController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var persons = _context.Persons.ToList();
            return View(persons);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn file để upload!");
                return RedirectToAction(nameof(Index));
            }

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                ModelState.AddModelError("", "Vui lòng chọn file Excel (.xls, .xlsx)!");
                return RedirectToAction(nameof(Index));
            }

            // Tạo thư mục lưu file tạm nếu chưa có
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Models", "Uploads", "Excels");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Đặt tên file duy nhất
            var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                // Lưu file Excel lên server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Đọc dữ liệu Excel
                DataTable dt = _excelProcess.ExcelToDataTable(filePath);
                if (dt == null || dt.Rows.Count == 0)
                {
                    ModelState.AddModelError("", "File không có dữ liệu hoặc định dạng sai!");
                    return RedirectToAction(nameof(Index));
                }

                // Import từng dòng vào DB
                int count = 0;
                foreach (DataRow row in dt.Rows)
                {
                    var ps = new Person
                    {
                        // map đúng cột
                        PersonId = row[0]?.ToString(),
                        FullName = row[1]?.ToString(),
                        Address = row[2]?.ToString(),
                        Email = row.Table.Columns.Count > 3 ? row[3]?.ToString() : null
                    };

                    // Nếu dòng bị trống, bỏ qua
                    if (string.IsNullOrWhiteSpace(ps.FullName))
                        continue;

                    await _context.Persons.AddAsync(ps);
                    count++;

                    if (count % 100 == 0)
                        await _context.SaveChangesAsync(); // lưu theo batch
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = $"Đã import {count} bản ghi từ file Excel.";

                // Xóa file sau khi dùng xong (tùy chọn)
                // System.IO.File.Delete(filePath);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi upload: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------- CREATE ----------------
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonId,FullName,Address,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // ---------------- EDIT ----------------
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonId,FullName,Address,Email")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            return View(person);
        }

        // ---------------- DELETE ----------------
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

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


        private bool PersonExists(string id)
        {
            return _context.Persons.Any(e => e.PersonId == id);
        }
    }
}


