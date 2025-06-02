using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.MyCustomValidation
{
    public class FileExtensionAttribute: ValidationAttribute
    {
        private readonly string[] _extensionsArr;
        public FileExtensionAttribute(string[] extensions)
        {
            _extensionsArr = extensions;
            ErrorMessage = $"Error:Allowed file types are: {string.Join(", ", extensions)}";
        }

        public override bool IsValid(object obj)
        {
            if (obj is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensionsArr.Contains(extension))
                {
                    return false;
                }
            }
            return true;
        }
    }
    
}
