using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;
        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex, string? search)
        {
            IQueryable<Product> query = context.Products;
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Author.Contains(search));
            }
            query = query.OrderByDescending(p => p.Id);
            if (pageIndex < 1) 
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages= (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex -1 )* pageSize).Take(pageSize);
         //   var product = context.Products.OrderByDescending(p=>p.Id).ToList();

            var products = query.ToList();
            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            ViewData["Search"] = search ?? "";
            return View();
        }
        public ActionResult Details (int id)
        {
            var product = context.Products.Find(id);
            if (product == null) 
            {
                return RedirectToAction("Index", "Store");
            }
            return View(product);
        }
    }
}
