using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="*")]
        public string Username { get; set; }


        [Required(ErrorMessage ="*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="*")]
        [Display(Name ="Remember Me ?")]
        public bool RememberMe { get; set; } = false;
    }
}
