using Microsoft.AspNetCore.Mvc;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ToDoContext _context;

        public AccountController(ToDoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //Step 1: Register - GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Step 1: Register - POST
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }

            var newUser = new User { Username = username, Password = password };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index", "Home");
        }
    }
}
