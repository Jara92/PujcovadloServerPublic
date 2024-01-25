using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PujcovadloServer.AuthorizationHandlers.Exceptions;
using PujcovadloServer.Business.Exceptions;

namespace PujcovadloServer.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ForbiddenAccessException)
        {
            context.Result = new ForbidResult(context.Exception.Message);
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            context.Result = new UnauthorizedResult();
        }
        else if (context.Exception is EntityNotFoundException)
        {
            context.Result = new NotFoundResult();
        }
        else if (context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new ConflictResult();
        }
    }
}