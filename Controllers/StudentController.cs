using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIS_RubricFeedbackGenerator.Data;
using AIS_RubricFeedbackGenerator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AIS_RubricFeedbackGenerator.Areas.Identity.Data;

namespace AIS_RubricFeedbackGenerator.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly AIS_RubricFeedbackGeneratorContext _context;
        private readonly UserManager<AIS_RubricFeedbackGeneratorUser> _userManager;

        public StudentController(AIS_RubricFeedbackGeneratorContext context, UserManager<AIS_RubricFeedbackGeneratorUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.CreatedByNavigation)
                .OrderBy(s => s.FullName)
                .ToListAsync();
            return View(students);
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View(new StudentViewModel());
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if student ID already exists
                if (await _context.Students.AnyAsync(s => s.StudentId == model.StudentId))
                {
                    ModelState.AddModelError("StudentId", "A student with this ID already exists.");
                    return View(model);
                }

                var student = model.ToStudent();
                student.CreatedBy = _userManager.GetUserId(User);
                student.CreatedAt = DateTime.Now;
                
                _context.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var model = StudentViewModel.FromStudent(student);
            return View(model);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StudentViewModel model)
        {
            if (id != model.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = model.ToStudent();
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(model.StudentId))
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
            return View(model);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.CreatedByNavigation)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}