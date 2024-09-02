using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Controllers
{
	public class HomeController : Controller
	{
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
		{
            this.context = context;
        }

		public IActionResult Index()
		{
			var product = context.Products.OrderByDescending(p=>p.Id).Take(4).ToList();
			return View(product);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
