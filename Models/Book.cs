using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.MyCustomValidation;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
       
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }//navigation property

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Book Genre")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } //navigation property

        [DateNotInTheFuture(ErrorMessage = "Published date cannot be in the future")]
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        
        public string Description { get; set; }
        public string? ImagePath { get; set; }

        // New properties for book status and borrowing
        public bool IsAvailable { get; set; } = true;
        public DateTime? BorrowedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? BorrowedBy { get; set; }

        //user book borrowing details
        public string? BorrowedByUserId { get; set; } 
        //user book relationship
        public virtual ApplicationUser? BorrowedByUser { get; set; }
    }
}
