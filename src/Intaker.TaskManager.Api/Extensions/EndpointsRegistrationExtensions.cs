using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Queries;
using MediatR;
using Intaker.TaskManager.Api.Routes;

namespace Intaker.TaskManager.Api.Extensions
{
    public static class EndpointsRegistrationExtensions
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapTaskRoutes();
            
            return app;
        }
    }
} 