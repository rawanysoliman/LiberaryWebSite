using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.MyCustomValidation
{
    public class CustomFullNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string fullName)
            {
                var parts = fullName.Split(' ');
                if (parts.Length == 2 && parts.All(p => p.Length >= 2))
                    return ValidationResult.Success;
            }
            return new ValidationResult("Full name must contain 2 words, each at least 2 characters.");
        }
    }
}