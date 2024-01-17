using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_pre_skolu.Models;

namespace Projekt_pre_skolu.Controllers
{
    public class StudentsController : Controller
    {
        private readonly Data.StudentDbContext _context;

        public StudentsController(Data.StudentDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }
            
            return _context.Student != null ?
                          View(await _context.Student.ToListAsync()) :
                          Problem("Entity set 'Projekt_pre_skoluContext.Student'  is null.");
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }

            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            var model = new StudentViewModel()
            {
                Id = student.Id,
                Email = student.Email,
                Mobil = student.Mobil,
                Name = student.Name,
                FavouriteSubjectId = student.FavouriteSubjectId,
                Surname = student.Surname,
                Image = student.ImageString
            };
            return View(model);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }
            var model = new StudentViewModel();
            model.Subjects = _context.Subject.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.Subjects.Add(new SelectListItem() { Selected = true, Value = "", Text = "" });
            return View(model);
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Mobil,Email,FavouriteSubjectId")] Student student, IFormFile? image)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }
            if (ModelState.IsValid)
            {
                if (image?.Length > 0)
                {
                    using var ms = new MemoryStream();
                    image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    student.ImageString = Convert.ToBase64String(fileBytes);
                }

                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }

            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel()
            {
                Id = student.Id,
                Email = student.Email,
                Mobil = student.Mobil,
                Name = student.Name,
                FavouriteSubjectId = student.FavouriteSubjectId,
                Surname = student.Surname,
                Image = student.ImageString
            };
            model.Subjects = _context.Subject.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.Subjects.Add(new SelectListItem() { Selected = true, Value = "", Text = "" });
            return View(model);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Surname,Mobil,Email,FavouriteSubjectId")] Student student, string? OldImage, IFormFile? image)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }

            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image?.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        image.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        student.ImageString = Convert.ToBase64String(fileBytes);
                    }
                    else
                    {
                        student.ImageString = OldImage;
                    }

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }

            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel()
            {
                Id = student.Id,
                Email = student.Email,
                Mobil = student.Mobil,
                Name = student.Name,
                FavouriteSubjectId = student.FavouriteSubjectId,
                Surname = student.Surname
            };
            return View(model);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToLogIn();
            }

            if (_context.Student == null)
            {
                return Problem("Entity set 'Projekt_pre_skoluContext.Student'  is null.");
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
            return (_context.Student?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool IsLoggedIn()
        {
            var username = HttpContext.Session.GetString("username");
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }
            return true;
        }

        private IActionResult RedirectToLogIn()
        {
            return RedirectToAction("Index", "Account", new UserViewModel { Message = "You have to log in to access this site.", IsError = true });
        }
    }
}
