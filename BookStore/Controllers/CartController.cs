using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal shippingFee;

        public CartController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto model) 
        {
            List<OrderItem>  cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (cartItems.Count == 0) 
            {
                ViewBag.ErrorMessage = "Your cart is empty.";
                return View(model);
            }
            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;
            return RedirectToAction("Confirm");
           
        }
        public IActionResult Confirm() 
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubtotal(cartItems) + shippingFee;
            int cartSize = 0;
            foreach(var item in cartItems) {
                cartSize += item.Quantity; 
            }

            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod =   TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0) 
            {
                return RedirectToAction("Index", "Home");   
            }
            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.CartSize = cartSize;
            ViewBag.Total = total;
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(string? any)
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, context);
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null) 
            {
                return RedirectToAction("Index", "Home");
            }
            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                PaymentDetails = "",
                OrderStatus = "Pending",
                CreatedAt = DateTime.Now
                
            };

            context.Orders.Add(order);
            context.SaveChanges();

            Response.Cookies.Delete("shopping_cart");

            ViewBag.SuccessMessage = "Order created Successfully";
            return View();
        }
    }
}
