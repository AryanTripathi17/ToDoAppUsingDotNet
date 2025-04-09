using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;

        public HomeController(ToDoContext ctx) => context = ctx;

        // Private helper method to check login
        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        public IActionResult Index(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var username = HttpContext.Session.GetString("Username");
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            var filters = new Filters(id);
            ViewBag.Filters = filters;

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Status.ToList();
            ViewBag.DueFilterValues = Filters.DueFilterValues;

            IQueryable<ToDo> query = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Where(t => t.UserId == user.Id); //Only show tasks for logged-in user

            if (filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }

            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Status.ToList();

            // Pre-select "open" status
            var task = new ToDo
            {
                StatusId = "open"
            };

            return View(task);
        }

        [HttpPost]
        public IActionResult Add(ToDo task)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var username = HttpContext.Session.GetString("Username");
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                task.UserId = user.Id; //Assign current user to new task
                context.ToDos.Add(task);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Status.ToList();
                return View(task);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var id = string.Join("-", filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            selected = context.ToDos.Find(selected.Id);
            if (selected != null)
            {
                selected.StatusId = "closed";
                context.SaveChanges();
            }
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var username = HttpContext.Session.GetString("Username");
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Account");

            var toDelete = context.ToDos
                .Where(t => t.StatusId == "closed" && t.UserId == user.Id) //Only delete this user's tasks
                .ToList();

            foreach (var task in toDelete)
            {
                context.ToDos.Remove(task);
            }

            context.SaveChanges();
            return RedirectToAction("Index", new { ID = id });
        }
    }
}
