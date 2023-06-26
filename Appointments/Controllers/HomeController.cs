using Appointments.Data;
using Appointments.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;

namespace Appointments.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Datacontext _context;

        public HomeController(Datacontext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            string? loggedin = "";

            if (TempData["loggedin"] != null)
            {
                loggedin = TempData["loggedin"] as string;
            }
            
            if (loggedin != "success")
            {
                return RedirectToAction(nameof(Login));
            }

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "Description_desc" : "Description";
            ViewData["BeginDateSortParm"] = sortOrder == "BeginDate" ? "BeginDate_desc" : "BeginDate";
            ViewData["EndDateSortParm"] = sortOrder == "EndDate" ? "EndDate_desc" : "EndDate";
            ViewData["CurrentFilter"] = searchString;
            var appointment = from s in _context.Appointment
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                appointment = appointment.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    appointment = appointment.OrderByDescending(s => s.Title);
                    break;
                case "Description":
                    appointment = appointment.OrderBy(s => s.Description);
                    break;
                case "Description_desc":
                    appointment = appointment.OrderByDescending(s => s.Description);
                    break;
                case "BeginDate":
                    appointment = appointment.OrderBy(s => s.BeginDate);
                    break;
                case "BeginDate_desc":
                    appointment = appointment.OrderByDescending(s => s.BeginDate);
                    break;
                case "EndDate":
                    appointment = appointment.OrderBy(s => s.EndDate);
                    break;
                case "EndDate_desc":
                    appointment = appointment.OrderByDescending(s => s.EndDate);
                    break;
                default:
                    appointment = appointment.OrderBy(s => s.Title);
                    break;
            }
            return View(await appointment.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var User = from m in _context.User select m;
                User = User.Where(s => s.Username.Contains(model.Username));
                if (User.Count() != 0)
                {
                    if (User.First().Password == model.Password)
                    {
                        if (User.First().Email == model.Email)
                        {
                            TempData["loggedin"] = "success";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            TempData["loggedin"] = "failed";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            TempData["loggedin"] = "loggedout";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Registrate()
        {
            return View();
        }
        
        public IActionResult Registrated()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrate(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Registrated));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Edit(int id, Appointment appointment)
        {
            ViewBag.ID = id;

            var appointmentToUpdate = _context.Appointment.FirstOrDefault(s => s.ID == id);

            return View(appointmentToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Appointment appointment, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointmentToUpdate = await _context.Appointment.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Appointment>(appointmentToUpdate,
                "",
                s => s.Title, s => s.Description, s => s.BeginDate, s => s.EndDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        public async Task<IActionResult> Deleteing(Appointment appointment, int id)
        {
            var appointmentdelete = await _context.Appointment.FindAsync(id);
            if (appointmentdelete == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Appointment.Remove(appointmentdelete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Index), new { id = id, saveChangesError = true });
            }
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}