using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
	[Authorize(Roles ="admin")]
	[Route("/Admin/[controller]/{action=Index}/{id?}")]
	public class ProductsController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;
		private readonly int pageSize = 5;

		public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
		{
			this.context = context;
			this.environment = environment;
		}
		public IActionResult Index(int pageIndex, string? search,string? column, string? orderBy)
		{ 

			IQueryable<Product> query = context.Products;
			if (search != null)
			{
				query = query.Where(p => p.Name.Contains(search) || p.Author.Contains(search));
			}

			//sort functionality

			string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
			string[] validOrderBy = { "desc", "asc" };

			if (!validColumns.Contains(column))
			{
				column = "Id";
			}
			if (!validOrderBy.Contains(orderBy)) 
			{
				orderBy = "desc";
			}
			//query = query.OrderByDescending(p => p.Id);

			if (column == "Name")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Name);
				}
				else
				{
					query = query.OrderByDescending(p => p.Name);
				}
			}
			else if (column == "Author")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Author);
				}
				else
				{
					query = query.OrderByDescending(p => p.Author);
				}
			}
			else if (column == "Category")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Category);
				}
				else
				{
					query = query.OrderByDescending(p => p.Category);
				}
			}
			else if (column == "Price")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Price);
				}
				else
				{
					query = query.OrderByDescending(p => p.Price);
				}
			}
			else if (column == "CreatedAt")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.CreatedAt);
				}
				else
				{
					query = query.OrderByDescending(p => p.CreatedAt);
				}
			}
			else {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }
            if (pageIndex < 1)
			{
				pageIndex = 1;
			}
			decimal count = query.Count();
			int totolPages= (int)Math.Ceiling(count/pageSize);
			query = query.Skip((pageIndex-1)*pageSize).Take(pageSize);
            var products = query.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPage"] = totolPages;
            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;
            return View(products);
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(ProductDto productDto)
		{
			if (productDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The Image file is required.");
			}
			if (!ModelState.IsValid)
			{
				return View(productDto);
			}
			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

			string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
			using (var strem = System.IO.File.Create(imageFullPath))
			{
				productDto.ImageFile.CopyTo(strem);
			}

			Product product = new Product()
			{
				Name = productDto.Name,
				Author = productDto.Author,
				Category = productDto.Category,
				Price = productDto.Price,
				Description = productDto.Description,
				ImageFileName = newFileName,
				CreatedAt = DateTime.Now
			};

			context.Products.Add(product);
			context.SaveChanges();
			return RedirectToAction("Index", "Products");
		}

		public IActionResult Edit(int id)
		{
			var product = context.Products.Find(id);
			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}
			var productDto = new ProductDto()
			{
                Name = product.Name,
                Author = product.Author,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

			ViewData["ProductId"] = product.Id;
			ViewData["ImageFileName"] = product.ImageFileName;
			ViewData["CreateAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
			return View();
		}
		[HttpPost]
		public IActionResult Edit(int id, ProductDto productDto)
		{
			var product = context.Products.Find(id);
			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}

			if (!ModelState.IsValid)
			{
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

				return View(productDto);
            }

			string newFileName = product.ImageFileName;

			if (productDto.ImageFile != null) 
			{
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

                string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
                using (var strem = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(strem);
                }

				string oldImageFullPath = environment.WebRootPath + "/Products/" + product.ImageFileName;

				System.IO.File.Delete(imageFullPath);
            }
			

			product.Name = productDto.Name;
			product.Author = productDto.Author;
			product.Category = productDto.Category;
			product.Price = productDto.Price;
            product.Description = productDto.Description;
			product.ImageFileName = newFileName;

			context.SaveChanges();
			return RedirectToAction("Index", "Products");



        }

		public IActionResult Delete(int id) 
		{
			var product = context.Products.Find(id);
			if (product == null)
			{
				return RedirectToAction("Index", "Products");
			}

			var imageFullPath = environment.WebRootPath + "/Products/" + product.ImageFileName;
			System.IO.File.Delete(imageFullPath);

			context.Products.Remove(product);
			context.SaveChanges();

			return RedirectToAction("Index", "Products");

		}


    }    
}
