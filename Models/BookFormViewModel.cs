using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.MyCustomValidation;

namespace LibraryManagementSystem.Models
{
    public class BookFormViewModel
    {
        public Book Book { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Category> Categories { get; set; }


        [FileExtension(new string[] { ".jpg", ".jpeg", ".png" })]
        public IFormFile ImageFile { get; set; } // For image upload


        public bool IsEditMode { get; set; } = false;

    }
}
