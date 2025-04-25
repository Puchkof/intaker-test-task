using Intaker.TaskManager.Application.Commands;
using Intaker.TaskManager.Application.DTOs;
using Intaker.TaskManager.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Intaker.TaskManager.Api.Routes
{
    public static class TaskRoutes
    {
        public static IEndpointRouteBuilder MapTaskRoutes(this IEndpointRouteBuilder app)
        {
            // Task Management API routes
            var tasksGroup = app.MapGroup(ApiRoutes.Tasks.Base)
                .WithTags("Tasks") // Add a tag for Swagger organization
                .WithOpenApi();
            
            tasksGroup.MapGet("/", GetAllTasks)
                .WithName("GetTasks")
                .Produces<IEnumerable<TaskItemDto>>(StatusCodes.Status200OK);
            
            tasksGroup.MapGet("/{id:int}", GetTaskById)
                .WithName("GetTaskById")
                .Produces<TaskItemDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            tasksGroup.MapPost("/", CreateTask)
                .WithName("CreateTask")
                .Produces<TaskItemDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            tasksGroup.MapPatch("/{id:int}/status", UpdateTaskStatus)
                .WithName("UpdateTaskStatus")
                .Produces<TaskItemDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
            
            return app;
        }

        private static async Task<IResult> GetAllTasks(IMediator mediator)
        {
            var result = await mediator.Send(new GetTasksQuery());
            return Results.Ok(result);
        }

        private static async Task<IResult> GetTaskById(int id, IMediator mediator)
        {
            var result = await mediator.Send(new GetTaskByIdQuery(id));
            return Results.Ok(result);
        }

        private static async Task<IResult> CreateTask(CreateTaskDto dto, IMediator mediator)
        {
            var command = new AddTaskCommand(dto.Name, dto.Description);
            var result = await mediator.Send(command);
            return Results.CreatedAtRoute("GetTaskById", new { id = result.Id }, result);
        }

        private static async Task<IResult> UpdateTaskStatus(int id, UpdateTaskStatusDto dto, IMediator mediator)
        {
            var command = new UpdateTaskStatusCommand(id, dto.Status);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }
    }
}