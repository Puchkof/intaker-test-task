using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Intaker.TaskManager.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred during request processing");
            
            httpContext.Response.ContentType = "application/json";
            
            object responseObj;
            
            // Customize response based on exception type
            switch (exception)
            {
                case ValidationException validationException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseObj = new
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Message = "Validation failed",
                        Errors = validationException.Errors
                    };
                    break;
                    
                case InvalidOperationException invalidOpException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseObj = new
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Message = invalidOpException.Message
                    };
                    break;
                    
                case ArgumentException argumentException:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseObj = new
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Message = argumentException.Message
                    };
                    break;
                    
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseObj = new
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Message = "An error occurred while processing your request."
                    };
                    break;
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var json = JsonSerializer.Serialize(responseObj, options);
            
            await httpContext.Response.WriteAsync(json, cancellationToken);
            
            return true;
        }
    }
} 