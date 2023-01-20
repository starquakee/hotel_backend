using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
//using static HotelManagement.ErrorHandling.BackendException;
namespace HotelManagement.ErrorHandling
{
    public class BackendExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
           if(context.Exception is BackendException backendException)
            {
                context.Result = new ObjectResult(backendException.ToJson())
                {
                    StatusCode = 200
                };
            }
            else
            {
                /*var exception = new BackendException
                {
                    Code = 10086,
                    ErrorMessage = "unknown"
                };

               context.Result = new ObjectResult(exception.ToJson())
               {
                    StatusCode = 200
                };*/
            }
        }
    }
}
