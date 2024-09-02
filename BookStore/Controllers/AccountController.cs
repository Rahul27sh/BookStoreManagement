using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User)) 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) 
            {
                return View(registerDto);
            }

            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt= DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(user,registerDto.Password);
            if (result.Succeeded) {
                //successful user registeration
                await userManager.AddToRoleAsync(user, "client");

                //sign in the new user
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors) {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);
        }

        public async Task<IActionResult> Logout() 
        {
            if (signInManager.IsSignedIn(User)) 
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index","Home");
        }
        public IActionResult Login() 
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid) 
            {
                return View(loginDto);
            }
            
            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,loginDto.RememberMe,false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                ViewBag.ErrorMessage = "Invlid login attept";
            }
            return View();
        }
        [Authorize]
        public IActionResult Profile() 
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
