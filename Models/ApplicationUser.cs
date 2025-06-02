using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models
{
    //this is the user model for the application which is inherited from the IdentityUser class
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        //every user can borrow many books
        //add navigation property for the books borrowed by the user
        public virtual ICollection<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}
