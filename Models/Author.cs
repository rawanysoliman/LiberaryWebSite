using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.MyCustomValidation;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [CustomFullNameValidation] // Custom validation for four names
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Website { get; set; }

        [MaxLength(300)]
        public string? Bio { get; set; }

        public virtual IReadOnlyCollection<Book> Books { get; set; } = new List<Book>();
    }
}
