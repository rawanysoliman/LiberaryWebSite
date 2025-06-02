using System.Security.Claims;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {

          //repo through which i deal with user table
        private UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManger;

        ////constructor injection
        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser>_signinManger)
        {

            userManager = _userManager;
            signinManger = _signinManger;
        }



        public IActionResult Register()
        {
            return View();
        }

        //Register as Admin 
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //mapping
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = vm.Usrername,
                    Email = vm.Email,
                    PasswordHash = vm.Password,
                    Address = vm.Address
                };
                //save to db
                var result= await userManager.CreateAsync(user, vm.Password);
                if(result.Succeeded)
                {
                    //assign role user by default
                    await userManager.AddToRoleAsync(user, "User");
                    //create cookie
                    await signinManger.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Books");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                } 
            }
            return View("Register",vm);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Login(LoginViewModel vm)
        {
            if (ModelState.IsValid) 
            {
                //chech found or not
              ApplicationUser appUser= await userManager.FindByNameAsync(vm.Username);
                if (appUser!=null)
                {
                  bool found=  await userManager.CheckPasswordAsync(appUser, vm.Password);

                    if(found==true)
                    {
                        //give cookie
                        await signinManger.SignInAsync(appUser, isPersistent: vm.RememberMe);
                        
                        return RedirectToAction("Index", "Books");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password"); 
            }
            return View("login",vm);
        }

        public async Task<IActionResult> Logout()
        {
            await signinManger.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
