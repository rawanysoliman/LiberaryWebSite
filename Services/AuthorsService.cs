using LibraryManagementSystem.Models;
using LibraryManagementSystem.UnitOfWorkPattern;





namespace LibraryManagementSystem.Services
{
    public interface IAuthorsService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> GetAuthorByIdAsync(int id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? e=null);
        Task<bool> IsEmailUniqueAsync(string email, int? e=null);
    }

    public class AuthorsService : IAuthorsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _unitOfWork.Authors.GetAll();
        }

        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            return await _unitOfWork.Authors.GetById(id);
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            await _unitOfWork.Authors.Add(author);
            await _unitOfWork.CommitChanges();
            return author;
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            var existingAuthor = await _unitOfWork.Authors.GetById(author.Id);
            if (existingAuthor != null)
            {
                existingAuthor.Name = author.Name;
                existingAuthor.Email = author.Email;
                existingAuthor.Website = author.Website;
                existingAuthor.Bio = author.Bio;

                _unitOfWork.Authors.Update(existingAuthor);
                await _unitOfWork.CommitChanges();
            }
            return existingAuthor;
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetById(id);
            if (author != null)
            {
                _unitOfWork.Authors.Remove(author);
                await _unitOfWork.CommitChanges();
            }
        }

        public async Task<bool> IsNameUniqueAsync(string name,int? excludedId=null)
        {
            return await _unitOfWork.Authors.IsUnique(a => a.Name == name 
            && (!excludedId.HasValue||a.Id!=excludedId.Value));
        }
        public async Task<bool> IsEmailUniqueAsync(string email, int? excludedId = null)
        {
            return await _unitOfWork.Authors.IsUnique(a => a.Email == email
            && (!excludedId.HasValue||a.Id!=excludedId.Value));
        }






    }
}
