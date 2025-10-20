using FitnessProject.Data;
using FitnessProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainerDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TrainerDetailsController(ApplicationDbContext context,
                                        UserManager<ApplicationUser> userManager,
                                        RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: TrainerDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrainerDetails.ToListAsync());
        }

        // GET: TrainerDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.TrainerDetails
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // GET: TrainerDetails/Create
        // GET: TrainerDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainerDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerDetails model)
        {
            Console.WriteLine("Create Trainer POST called");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                return View(model);
            }

            // Create the ApplicationUser
            var user = new ApplicationUser
            {
                UserName = model.Contact,
                Email = model.Contact,
                OTPCode = "",
                OTPExpiry = null
            };

            string defaultPassword = "Password@123";
            Console.WriteLine($"Attempting to create user with username/email: {model.Contact}");

            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (!result.Succeeded)
            {
                Console.WriteLine("User creation failed:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($" - {error.Code}: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            Console.WriteLine("User created successfully");

            // Assign Trainer role
            await _userManager.AddToRoleAsync(user, "Trainer");
            Console.WriteLine("Assigned Trainer role to user");

            // Link TrainerDetails to the newly created user
            model.ApplicationUserId = user.Id;

            // Save TrainerDetails
            _context.TrainerDetails.Add(model);
            await _context.SaveChangesAsync();
            Console.WriteLine("TrainerDetails saved to database");

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainerDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.TrainerDetails.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: TrainerDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerDetails trainerDetails)
        {
            if (id != trainerDetails.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainerDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerDetailsExists(trainerDetails.Id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainerDetails);
        }

        // GET: TrainerDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.TrainerDetails
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: TrainerDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.TrainerDetails.FindAsync(id);
            _context.TrainerDetails.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerDetailsExists(int id)
        {
            return _context.TrainerDetails.Any(e => e.Id == id);
        }
    }
}
