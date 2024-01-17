using Microsoft.AspNetCore.Mvc;
using Projekt_pre_skolu.Data;
using Projekt_pre_skolu.Models;

namespace Projekt_pre_skolu.Controllers
{
    public class AccountController : Controller
    {
        private readonly Data.StudentDbContext _context;

        public AccountController(Data.StudentDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(UserViewModel? model = null)
        {
            var user = HttpContext.Session.GetString("username");
            if (!string.IsNullOrWhiteSpace(user))
            {
                return RedirectToHome();
            }
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login([Bind("Username,Password")] User user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new UserViewModel() { Message = "Username or password can not be empty.", IsError = true });
            }
            var userFromDB = _context.User.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password));
            if (userFromDB == null)
            {
                return RedirectToAction("Index", new UserViewModel() { Message = "Username or password is wrong.", IsError = true, Username = user.Username });
            }
            HttpContext.Session.SetString("username", user.Username);
            return RedirectToHome();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }

        public IActionResult Registration(UserViewModel model)
        {
            var user = HttpContext.Session.GetString("username");
            if (!string.IsNullOrWhiteSpace(user))
            {
                return RedirectToHome();
            }
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser([Bind("Username,Password")] User user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Registration", new UserViewModel() { Message = "Username or password can not be empty.", IsError = true });
            }
            var userFromDB = _context.User.Where(x => x.Username.Equals(user.Username));
            if (userFromDB?.Any() == true)
            {
                return RedirectToAction("Registration", new UserViewModel() { Message = "This username is already in use.", IsError = true, Username = user.Username });
            }
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new UserViewModel() { Message = "User was created. Now you can log in.", IsError = false, Username = user.Username });
        }

        private IActionResult RedirectToHome()
        {
            return RedirectToAction("Index", "Students");
        }
    }
}
