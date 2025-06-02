using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        public RoleController(RoleManager<IdentityRole> _rolemngr,UserManager<ApplicationUser> _usermngr )
        {
            roleManager = _rolemngr;
            userManager = _usermngr;
        }
        public IActionResult CreateRole()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateRole(String roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = roleName
                };
                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Role created successfully.";
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("createrole");
        }

        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }
        


    

        public async Task<IActionResult> AssignRole()
        {
            // First get all users (one connection)
            var users = await userManager.Users.ToListAsync();

            //check that user has only one role
            foreach (var user in users)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                if (userRoles.Count > 1)
                {
                    ModelState.AddModelError("", "User has more than one role.");
                }
            }   
            //add the users and roles to the view model
            AssignRoleViewModel vm = new AssignRoleViewModel
            {
                Users = userManager.Users.Select(u => new SelectListItem
                {
                    Text = u.UserName,
                    Value = u.Id
                }).ToList(),
                Roles = roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }).ToList()
            };
            return View(vm);
        }




        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //get the user
                var user = await userManager.FindByIdAsync(vm.Username);
                if (user != null)
                {
                    //get the current role
                    var currentRole = await userManager.GetRolesAsync(user);
                    if (currentRole.Contains(vm.Role))
                    {
                        ModelState.AddModelError("", "User already has this role.");
                        return View(vm);
                    }
                    //remove the existing role
                    if(currentRole.Any())
                    {
                        var removeRole = await userManager.RemoveFromRolesAsync(user, currentRole);
                        if (!removeRole.Succeeded)
                        {
                            foreach (var error in removeRole.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    //add the new role
                    var result = await userManager.AddToRoleAsync(user, vm.Role);
                    if (result.Succeeded)
                    {
                        ViewBag.Message = $"Role {vm.Role} assigned to user {vm.Username}.";
                        return RedirectToAction("ListRoles");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            //return the  listRoles view
            return RedirectToAction("ListRoles");
        }
    }
}
