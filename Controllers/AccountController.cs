using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AccountController : Controller
    {

        private RoleManager<IdentityRole> roleManager;  //repo through which i deal with role table
        private UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManger;

        //        //constructor injection
        public AccountController(RoleManager<IdentityRole> _rolemanager, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser>_signinManger)
        {
            roleManager = _rolemanager;
            userManager = _userManager;
            signinManger = _signinManger;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

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

        public IActionResult Logout()
        {
            signinManger.SignOutAsync();
            return View("Login");
        }


    }
}
