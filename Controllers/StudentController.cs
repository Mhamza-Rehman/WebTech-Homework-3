using AttendanceSystem.Models;
using AttendanceSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentRepository _repository;

        public StudentController(StudentRepository repository)
        {
            _repository = repository;
        }

        // GET: /Student
        public async Task<IActionResult> Index()
        {
            var students = await _repository.GetAllAsync();
            return View(students);
        }

        // GET: /Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            await _repository.AddAsync(student);
            TempData["Message"] = "Student added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // POST: /Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(student);

            bool updated = await _repository.UpdateAsync(student);
            if (!updated)
                return NotFound();

            TempData["Message"] = "Student updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _repository.DeleteAsync(id);
            TempData["Message"] = deleted ? "Student deleted successfully." : "Student not found.";
            return RedirectToAction(nameof(Index));
        }
    }
}