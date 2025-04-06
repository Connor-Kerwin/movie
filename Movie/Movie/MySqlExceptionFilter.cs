using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Movie.Models;
using MySqlConnector;

namespace Movie;

public class MySqlExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        // NOTE: The two observed exception cases are MySqlException and an InvalidOperationException wrapping a MySqlException

        if (context.Exception is MySqlException sqlEx)
        {
            HandleDatabaseException(sqlEx, context);
        }

        if (context.Exception is InvalidOperationException invalidEx &&
            invalidEx.InnerException is MySqlException innerSqlEx)
        {
            HandleDatabaseException(innerSqlEx, context);
        }
    }

    private static void HandleDatabaseException(MySqlException ex, ExceptionContext context)
    {
        // NOTE: There may be more specific error codes we want to categorize into the unavailable category
        
        if (ex.ErrorCode == MySqlErrorCode.UnableToConnectToHost)
        {
            var response = new ErrorModel
            {
                Message = "Database unavailable"
            };
            
            context.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Result = new JsonResult(response);
            

        }
        else // Other errors indicate that something else went wrong
        {
            var response = new ErrorModel
            {
                Message = "A database error occurred"
            };
            
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(response);
        }
        
        context.ExceptionHandled = true;
    }
}