using LibraryManagementSystem.Models;
using LibraryManagementSystem.MyCustomValidation;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;  


//a controller for the books
//methods included:
//Index,Details,Add,Edit,Delete,DeleteConfirmed

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: Books
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksWithDetails();
            return View(books);
        }



        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdWithDetails(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        //get the add form
        public async Task<IActionResult> Add()
        {
            var vm = await _bookService.GetBookFormViewModel();
            return View(vm);
        }

        #region HELPER METHODS
        // private async Task GetWithAuthorsAndCategories(BookFormViewModel vm)
        // {
        //     vm.Authors = await _bookService.GetAllAuthors();
        //     vm.Categories = await _bookService.GetAllCategories();
        // }

        private async Task<string> UploadImage(IFormFile imageFile)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploads, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/Images/" + uniqueFileName;
        }

        private void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }
        #endregion

        [MaxImageSizeFilter(5 * 1024 * 1024)]
        [HttpPost]
        public async Task<IActionResult> Add(BookFormViewModel vm, IFormFile ImageFile)
        {
            bool isTitleUnique = await _bookService.IsTitleUnique(vm.Book.Title);
            if (!isTitleUnique)
            {
                ModelState.AddModelError("Book.Title", "Book title must be unique.");
            }

            if (!vm.IsEditMode && (vm.ImageFile == null || vm.ImageFile.Length == 0))
            {
                ModelState.AddModelError(nameof(vm.ImageFile), "Image is required");
            }
            
            if (!ModelState.IsValid)
            {
                vm = await _bookService.GetBookFormViewModel();
                return View(vm);

                // var formVm = await _bookService.GetBookFormViewModel(vm.Book.Id);
                // vm.Authors = formVm.Authors;
                // vm.Categories = formVm.Categories;
                //  return View(vm);
            }

            vm.Book.ImagePath = await _bookService.UploadBookImage(vm.ImageFile);
            await _bookService.AddBook(vm.Book);

            return RedirectToAction("Index");
        }

        //get
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _bookService.GetBookFormViewModel(id);
            if (vm.Book == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [MaxImageSizeFilter(5 * 1024 * 1024)]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookFormViewModel vm, IFormFile? ImageFile)
        {
            bool isTitleUnique = await _bookService.IsTitleUnique(vm.Book.Title, vm.Book.Id);
            if (!isTitleUnique)
            {
                ModelState.AddModelError("Book.Title", "Book title must be unique.");
            }

            if (id != vm.Book.Id) return NotFound();
            vm.IsEditMode = true;

            if (!ModelState.IsValid)
            {
                vm = await _bookService.GetBookFormViewModel(id);
                return View(vm);
            }
            //if user added new mge file, delete the old one and upload the new one
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var existingBook = await _bookService.GetBookById(id);
                if (existingBook != null)
                {
                    _bookService.DeleteBookImage(existingBook.ImagePath);
                }
                vm.Book.ImagePath = await _bookService.UploadBookImage(vm.ImageFile);
            }
            else
            {
                // Keep the existing image path
                var existingBook = await _bookService.GetBookById(id);
                vm.Book.ImagePath = existingBook?.ImagePath;
            }

            await _bookService.UpdateBook(vm.Book);
            return RedirectToAction(nameof(Index));
        }

        //get the confirmation page
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdWithDetails(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        //form submission
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBook(id);
            return RedirectToAction("Index");
        }





        //******************************************************************
        // GET: Books/BookLibrary
        [Authorize]
        public async Task<IActionResult> BookLibrary(
            bool? filterAvailable,
            DateTime? filterBorrowedFrom,
            DateTime? filterBorrowedTo,
            DateTime? filterReturnedFrom,
            DateTime? filterReturnedTo)
        {
            var viewModel = await _bookService.GetBookLibraryViewModel(
                filterAvailable,
                filterBorrowedFrom,
                filterBorrowedTo,
                filterReturnedFrom,
                filterReturnedTo);
            return View(viewModel);
        }

        // GET: Books/BorrowBook/5
        [Authorize]
        public async Task<IActionResult> BorrowBook(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _bookService.GetBookByIdWithDetails(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            if (!book.IsAvailable)
            {
                TempData["ErrorMessage"] = "This book is already borrowed.";
                return RedirectToAction("BookLibrary");
            }

            return View(book);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowBook(int id)
        {
            //get the current user id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                //if the user is not logged in, redirect to the login page
                //and show a message
                TempData["ErrorMessage"] = "You must be logged in to borrow a book.";
                return RedirectToAction("Login", "Account");
            }

            //borrow the book
            var error = await _bookService.BorrowBook(id, userId);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("BookLibrary");
            }
            TempData["SuccessMessage"] = "Book borrowed successfully!";
            return RedirectToAction("BookLibrary");
        }

        // POST: Books/ReturnBook/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id)
        {
            //get the current user id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to return a book.";
                return RedirectToAction("BookLibrary");
            }
            //return the book
            var result = await _bookService.ReturnBook(id, userId);
            if (result != null)
            {
                TempData["ErrorMessage"] = result;      
                return RedirectToAction("BookLibrary");
            }

            TempData["SuccessMessage"] = "Book returned successfully!";
            return RedirectToAction("BookLibrary");
        }



//******************************************************************
        // GET: Books/NewBorrowBook
        public async Task<IActionResult> NewBorrowBook()
        {
            // Get all books with details
            var books = await _bookService.GetAllBooksWithDetails();
            return View(books);
        }

        // POST: Books/NewBorrowBook
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewBorrowBook(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var error = await _bookService.BorrowBook(id, userId);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("BookLibrary");
            }

            TempData["SuccessMessage"] = "Book borrowed successfully!";
            return RedirectToAction("BookLibrary");
        }

    }
}
