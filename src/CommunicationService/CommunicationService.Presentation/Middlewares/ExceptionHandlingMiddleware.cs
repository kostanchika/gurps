using CommunicationService.Application.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace UsersService.Presentation.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await HandleExceptionAsync(context, ex, JsonSerializer.Serialize(ex.Errors));
            }
            catch (UnauthorizedException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await HandleExceptionAsync(context, ex);
            }
            catch (ForbiddenOperationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await HandleExceptionAsync(context, ex);
            }
            catch (EntityNotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await HandleExceptionAsync(context, ex);
            }
            catch (ConflictOperationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), ex.ToString());

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new { context.Response.StatusCode, ex.Message });
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), ex.Message);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { context.Response.StatusCode, ex.Message });
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, string details)
        {
            _logger.LogError("An error of type {ExceptionType} occured: {Exception}", ex.GetType(), ex.Message);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { context.Response.StatusCode, ex.Message, details });
        }
    }
}
