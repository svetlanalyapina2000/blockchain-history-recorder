using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace BlockchainHistoryRecorder.Common.Exceptions;

public static class ExceptionHandler
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp
            => exceptionHandlerApp.Run(async context =>
            {
                var defaultExceptionResponse = Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
                await defaultExceptionResponse.ExecuteAsync(context);
            }));
    }

    public sealed class GlobalExceptionsHandler : IExceptionHandler
    {
        private static readonly IDictionary<Type, Func<HttpContext, Exception, IResult>> ExceptionHandlers =
            new Dictionary<Type, Func<HttpContext, Exception, IResult>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(InvalidOperationException), HandleInvalidOperationException },
                { typeof(ArgumentException), HandleArgumentException }
            };

        private readonly ILogger<GlobalExceptionsHandler> _logger;

        public GlobalExceptionsHandler(ILogger<GlobalExceptionsHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionMessage = exception.Message;
            _logger.LogError(
                "Error Message: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);

            var type = exception.GetType();

            if (!ExceptionHandlers.TryGetValue(type, out var handler)) return false;

            var result = handler.Invoke(httpContext, exception);
            await result.ExecuteAsync(httpContext);

            return true;
        }

        private static IResult HandleValidationException(HttpContext context, Exception exception)
        {
            return Results.Problem(exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        private static IResult HandleInvalidOperationException(HttpContext context, Exception exception)
        {
            return Results.Problem(exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        private static IResult HandleArgumentException(HttpContext context, Exception exception)
        {
            return Results.Problem(exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}