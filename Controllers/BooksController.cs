using LibraryManagementSystem.Models;
using LibraryManagementSystem.MyCustomValidation;
using LibraryManagementSystem.UnitOfWorkPattern;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        public IUnitOfWork _unitOfWork { get; }


        public BooksController(IUnitOfWork _IUnitOfWork)
        {
            _unitOfWork = _IUnitOfWork;
        }


        // GET: Books
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var books =await _unitOfWork.Books.GetAll();
            return View(books);
        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        #region HELPER METHODS
        private async Task GetWithAuthorsAndCategories(BookFormViewModel vm)
        {
            vm.Authors = await _unitOfWork.Authors.GetAll();
            vm.Categories = await _unitOfWork.Categories.GetAll();
        }

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

        //get the add form
        public async Task<IActionResult> Add()
        {
            //instead of viewbag
            BookFormViewModel vm = new BookFormViewModel()
            {
                Authors = await _unitOfWork.Authors.GetAll(),
                Categories= await _unitOfWork.Categories.GetAll(),
                Book=new Book()
            };
            return View(vm);
        }

        [MaxImageSizeFilter(5 * 1024 * 1024)]
        [HttpPost]
        public async Task<IActionResult> Add(BookFormViewModel vm , IFormFile ImageFile)
        {
            //check for uniqness
            bool isTitleUnique = await _unitOfWork.Books.IsUnique(b => b.Title == vm.Book.Title);
            if (!isTitleUnique)
            {
                ModelState.AddModelError("Book.Title", "Book title must be unique.");
            }
            //required validn. img when add only
            if (!vm.IsEditMode && (vm.ImageFile == null || vm.ImageFile.Length == 0))
            {
                ModelState.AddModelError(nameof(vm.ImageFile), "Image is required");
            }
            
            if (!ModelState.IsValid)
            {
                await  GetWithAuthorsAndCategories(vm);
                return View(vm);
            }
            //adding img file
            vm.Book.ImagePath = await UploadImage(vm.ImageFile);
            await _unitOfWork.Books.Add(vm.Book);
            await _unitOfWork.CommitChanges();

                return RedirectToAction("Index");
        }



        //get
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _unitOfWork.Books.GetById(id);

            if (book == null)
            {
                return NotFound();
            }
            //instead of viewbag
            BookFormViewModel vm = new BookFormViewModel()
            {
                Book = book,
                IsEditMode = true 
            };
            await GetWithAuthorsAndCategories(vm);
            return View(vm);
        }


        [MaxImageSizeFilter(5 * 1024 * 1024)]
        [HttpPost]

        public async Task<IActionResult> Edit(int id, BookFormViewModel vm, IFormFile? ImageFile)
        {
            //check for uniqness
            bool isTitleUnique = await _unitOfWork.Books.IsUnique(b => 
            b.Title == vm.Book.Title && b.Id != vm.Book.Id);

            if (!isTitleUnique)
            {
                ModelState.AddModelError("Book.Title", "Book title must be unique.");
            }
            if (id != vm.Book.Id) return NotFound();
            vm.IsEditMode = true;

            if (!ModelState.IsValid)
            {
                await GetWithAuthorsAndCategories(vm);
                return View(vm);
            }

            var existingBook = await _unitOfWork.Books.GetById(id);
                if (existingBook == null)
                    return NotFound();

                // reuse the original tracked entity, just update its properties
                existingBook.Title = vm.Book.Title;
                existingBook.AuthorId = vm.Book.AuthorId;
                existingBook.CategoryId = vm.Book.CategoryId;
                existingBook.Description = vm.Book.Description;
                existingBook.PublishedDate = vm.Book.PublishedDate;
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                // Delete old img if exists
                DeleteImage(existingBook.ImagePath);
                // Save new img
                existingBook.ImagePath =  await UploadImage(vm.ImageFile);
            }
            //else  keep old img
            //IUnitOfWork.Books.Update(existingBook); // no need updated manually
            await _unitOfWork.CommitChanges();

                return RedirectToAction(nameof(Index));
        }
        //get the confirmation page
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        //form submission
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book != null)
            {
                _unitOfWork.Books.Remove(book);
                //delete from root
                DeleteImage(book.ImagePath); 
                // Remove from the database
                await _unitOfWork.CommitChanges();
                return RedirectToAction("Index");

            }
            return NotFound();
        }

    }
}
