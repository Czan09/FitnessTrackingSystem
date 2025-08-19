using System.Diagnostics;
using FitnessProject.Data;
using FitnessProject.Models;
using FitnessProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly BulkAddTag _bulkAddTagService;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context,BulkAddTag bulkAddTagService)
        {
            _logger = logger;
            _context = context;
            _bulkAddTagService = bulkAddTagService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PrivacyAsync()
        {
            //await _bulkAddTagService.AddTagsToUserDetailsAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
