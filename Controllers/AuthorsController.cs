using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Models;




namespace LibraryManagementSystem.Controllers
{
    public class AuthorsController : Controller
    {

        private readonly IAuthorsService _authorsService;

        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _authorsService.GetAllAuthorsAsync();
            return View(authors);
        }
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorsService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            // Check uniqueness of name and email  
            if (!await _authorsService.IsNameUniqueAsync(author.Name))
            {
                ModelState.AddModelError("Name", "An author with this name already exists.");
            }
            if (!await _authorsService.IsEmailUniqueAsync(author.Email))
            {
                ModelState.AddModelError("Email", "An author with this email already exists.");
            }
            if (ModelState.IsValid)
            {
                await _authorsService.CreateAuthorAsync(author);
                return RedirectToAction("Index");
            }
            return View(author);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorsService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }
            // Check uniqueness of name and email  
            if (!await _authorsService.IsNameUniqueAsync(author.Name, author.Id))
            {
                ModelState.AddModelError("Name", "An author with this name already exists.");
            }
            if (!await _authorsService.IsEmailUniqueAsync(author.Email, author.Id))
            {
                ModelState.AddModelError("Email", "An author with this email already exists.");
            }

            if (ModelState.IsValid)
            {
                await _authorsService.UpdateAuthorAsync(author);
                return RedirectToAction("Index");
            }
            return View(author);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorsService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _authorsService.DeleteAuthorAsync(id);
            return RedirectToAction("Index");
        }
    }
}
