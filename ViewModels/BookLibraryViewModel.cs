using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.ViewModels
{
    public class BookLibraryViewModel
    {
        public IEnumerable<Book> Books { get; set; }
        //filtering parameters for the book library view by the user
        public bool? FilterAvailable { get; set; } //filter by availability
        public DateTime? FilterBorrowedFrom { get; set; } //filter by borrowed from
        public DateTime? FilterBorrowedTo { get; set; } //filter by borrowed to
        public DateTime? FilterReturnedFrom { get; set; } //filter by returned from
        public DateTime? FilterReturnedTo { get; set; } //filter by returned to
    }
} 