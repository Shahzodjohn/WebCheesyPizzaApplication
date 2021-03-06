using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCheesyPizzaApplication.Context;
using WebCheesyPizzaApplication.Models;
using WebCheesyPizzaApplication.ViewModels;

namespace WebCheesyPizzaApplication.Controllers
{
    public class SignController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;


        public SignController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl) => View(new LoginViewModel { returnUrl = ReturnUrl});
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByNameAsync(model.Login);
            if (user == null)
            {
                ModelState.AddModelError("Пользователь", "Не найден");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
            if (result.Succeeded)
                return Redirect(model.returnUrl ?? "/Products");
            ModelState.AddModelError("Login", "Login or password is incorrect");


            return View(model);
        }
        [HttpGet]
        public IActionResult SignUp() => View();
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new IdentityUser
            {
                UserName = model.Login,
                
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
               

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> DetailsUser(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var UserViewModel = new UserDetailViewModel
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                Login = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber
            };
            return View(UserViewModel);
        }
        public IActionResult Contacts()
        {
            return View();
        }
    }
}
