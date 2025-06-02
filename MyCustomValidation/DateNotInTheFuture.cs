using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.MyCustomValidation
{
    public class DateNotInTheFuture: ValidationAttribute
    {
        public DateNotInTheFuture()
        {
            ErrorMessage = "Error: Date cannot be in the future";
        }
        public override bool IsValid(object value)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                {
                    return false;
                }
            }
            return true;
        }
    }
   
}
