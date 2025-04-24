using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace LibraryManagementSystem.MyCustomValidation
{
    public class MaxImageSizeFilter: Attribute, IResourceFilter
    {
        private readonly long maxSizeBytes;

        public MaxImageSizeFilter(long _maxSizeBytes) 
        {
          maxSizeBytes=  _maxSizeBytes;
         }
        //dyring excution of the request
        public void OnResourceExecuting(ResourceExecutingContext cntxt)
        {
            var file = cntxt.HttpContext.Request.Form.Files.FirstOrDefault(f => f.Name == "ImageFile");

            if (file != null && file.Length > maxSizeBytes)
            {
                cntxt.ModelState.AddModelError("ImageFile", $"Image size must be less than {maxSizeBytes / 1024} KB.");
                cntxt.Result = new BadRequestObjectResult(cntxt.ModelState);
            }
        }
        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
