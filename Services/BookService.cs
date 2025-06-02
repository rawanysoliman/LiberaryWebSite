using LibraryManagementSystem.Models;
using LibraryManagementSystem.UnitOfWorkPattern;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

//a service layer for the books contains the business logic
//methods included:
//GetAllBooks,GetAllBooksWithDetails,GetBookById,GetBookByIdWithDetails,IsTitleUnique,GetBookFormViewModel,UploadBookImage,DeleteBookImage,AddBook,UpdateBook,DeleteBook
namespace LibraryManagementSystem.Services
{
    public interface IBookService
    {
        //methods for the book form view
        Task<IEnumerable<Book>> GetAllBooks();
        Task<IEnumerable<Book>> GetAllBooksWithDetails();
        Task<Book?> GetBookById(int id);
        Task<Book?> GetBookByIdWithDetails(int id);
        Task<bool> IsTitleUnique(string title, int? excludeId = null);
        Task<BookFormViewModel> GetBookFormViewModel(int? id = null);
        Task<string> UploadBookImage(IFormFile imageFile);
        void DeleteBookImage(string imagePath);
        Task AddBook(Book book);
        Task UpdateBook(Book book);
        Task DeleteBook(int id);

        //methods for the book library view
        //methode1: get the book library view model
        Task<BookLibraryViewModel> GetBookLibraryViewModel(
            bool? filterAvailable,
            DateTime? filterBorrowedFrom,
            DateTime? filterBorrowedTo,
            DateTime? filterReturnedFrom,
            DateTime? filterReturnedTo);
        //methode2: borrow a book
        Task<string?> BorrowBook(int id, string? borrowedByUserId);
        //methode3: return a book
        Task<string?> ReturnBook(int id, string? currentUserId);
    }




    public class BookService : IBookService
    {
        //inject the unit of work
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _unitOfWork.Books.GetAll();
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithDetails()
        {
            return await _unitOfWork.Books.GetAllIncluding(
                b => b.Author,
                b => b.Category
            );
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _unitOfWork.Books.GetById(id);
        }

        public async Task<Book?> GetBookByIdWithDetails(int id)
        {
            var books = await _unitOfWork.Books.GetAllIncluding(
                b => b.Author,
                b => b.Category
            );
            return books.FirstOrDefault(b => b.Id == id);
        }

        public async Task<bool> IsTitleUnique(string title, int? excludeId = null)
        {
            return await _unitOfWork.Books.IsUnique(b => 
                b.Title == title && (!excludeId.HasValue || b.Id != excludeId.Value));
        }


//use this methode to get the book form view model
        public async Task<BookFormViewModel> GetBookFormViewModel(int? id = null)
        {
            var viewModel = new BookFormViewModel
            {
                Authors = await _unitOfWork.Authors.GetAll(),
                Categories = await _unitOfWork.Categories.GetAll()
            };

            if (id.HasValue)
            {
                viewModel.Book = await _unitOfWork.Books.GetById(id.Value);
                viewModel.IsEditMode = true;
            }
            else
            {
                viewModel.Book = new Book();
            }

            return viewModel;
        }

        public async Task<string> UploadBookImage(IFormFile imageFile)
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

        public void DeleteBookImage(string imagePath)
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

        public async Task AddBook(Book book)
        {
            await _unitOfWork.Books.Add(book);
            await _unitOfWork.CommitChanges();
        }

        public async Task UpdateBook(Book book)
        {
            var existingBook = await _unitOfWork.Books.GetById(book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.AuthorId = book.AuthorId;
                existingBook.CategoryId = book.CategoryId;
                existingBook.Description = book.Description;
                existingBook.PublishedDate = book.PublishedDate;
                existingBook.ImagePath = book.ImagePath;

                _unitOfWork.Books.Update(existingBook);
                await _unitOfWork.CommitChanges();
            }
        }

        public async Task DeleteBook(int id)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book != null)
            {
                DeleteBookImage(book.ImagePath);
                _unitOfWork.Books.Remove(book);
                await _unitOfWork.CommitChanges();
            }
        }


//*************************************************************
        //methode1: get the book library view model
        public async Task<BookLibraryViewModel> GetBookLibraryViewModel(
            bool? filterAvailable, //filter by availability
            DateTime? filterBorrowedFrom, //filter by borrowed from
            DateTime? filterBorrowedTo, //filter by borrowed to
            DateTime? filterReturnedFrom, //filter by returned from
            DateTime? filterReturnedTo) //filter by returned to
        {
            //get all the books with their author and category to view card info
            var books = await _unitOfWork.Books.GetAllIncluding(
                b => b.Author,
                b => b.Category
            );

            // Apply filters
            if (filterAvailable.HasValue)
            {
                books = books.Where(b => b.IsAvailable == filterAvailable.Value);
            }

            if (filterBorrowedFrom.HasValue)
            {

                books = books.Where(b => b.BorrowedDate >= filterBorrowedFrom.Value);
            }

            if (filterBorrowedTo.HasValue)
            {
                books = books.Where(b => b.BorrowedDate <= filterBorrowedTo.Value);
            }

            if (filterReturnedFrom.HasValue)
            {
                books = books.Where(b => b.ReturnedDate >= filterReturnedFrom.Value);
            }

            if (filterReturnedTo.HasValue)
            {
                books = books.Where(b => b.ReturnedDate <= filterReturnedTo.Value);
            }

            return new BookLibraryViewModel
            {
                Books = books,
                FilterAvailable = filterAvailable,
                FilterBorrowedFrom = filterBorrowedFrom,
                FilterBorrowedTo = filterBorrowedTo,
                FilterReturnedFrom = filterReturnedFrom,
                FilterReturnedTo = filterReturnedTo
            };
        }
        

        public async Task<string?> BorrowBook(int id, string? borrowedByUserId)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book == null) return "Book not found.";
            if (!book.IsAvailable) return "This book is already borrowed.";

            book.IsAvailable = false;
            book.BorrowedDate = DateTime.Now;
            book.BorrowedByUserId = borrowedByUserId;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CommitChanges();

            return null; // success
        }

        public async Task<string?> ReturnBook(int id, string? currentUserId)
        {
            var book = await _unitOfWork.Books.GetById(id);
            if (book == null) return "Book not found.";
            if (book.IsAvailable) return "This book is already available.";
            
            // Check if the book is borrowed by the current user
            if (book.BorrowedByUserId != currentUserId)
            {
                return "This book is not borrowed by you.";
            }

            book.IsAvailable = true;
            book.ReturnedDate = DateTime.Now;
            book.BorrowedByUserId = null;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CommitChanges();

            return null; // success
        }



    }
}
