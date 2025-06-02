using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;

namespace LibraryManagementSystem.Models
{
    public class AssignRoleViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }

        public List<SelectListItem> Users { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}
