# Intaker Task Manager

A RESTful API for managing tasks, built using ASP.NET Core 8, PostgreSQL, and Azure Storage Queues.

## Features

- Create, view, and update task statuses
- Validation of task status transitions (tasks must follow a specific lifecycle)
- Message publishing to Azure Storage Queues when tasks are completed
- Background service for processing completed task messages
- Global error handling and logging
- Swagger documentation

## Technologies

- **.NET 8**: Latest version of .NET for modern, high-performance applications
- **ASP.NET Core Web API**: For building RESTful services
- **Entity Framework Core**: ORM for database operations
- **PostgreSQL**: Open-source relational database
- **Azure Storage Queues**: For message queuing and reliable event handling
- **MediatR**: For implementing CQRS pattern
- **FluentValidation**: For request validation
- **Swagger/OpenAPI**: For API documentation

## Project Structure

- **Intaker.TaskManager.Api**: Web API controllers and middleware
- **Intaker.TaskManager.Application**: Business logic, commands, and queries
- **Intaker.TaskManager.Domain**: Domain entities and logic
- **Intaker.TaskManager.Infrastructure**: Data access and messaging implementation
- **Intaker.TaskManager.Tests**: Unit and integration tests

## Requirements

- .NET 8 SDK
- Docker and Docker Compose (for containerized deployment)
- PostgreSQL (for local development without Docker)

## Setup Instructions

### Local Development

1. Clone the repository
2. Set up a local PostgreSQL instance or use Docker
3. Update connection strings in `appsettings.json` if needed
4. Run migrations to create the database schema:
   ```
   cd src/Intaker.TaskManager.Api
   dotnet ef database update
   ```
5. Run the application:
   ```
   dotnet run
   ```
6. Access the Swagger UI at `https://localhost:5001/swagger`

### Using Docker Compose

1. Clone the repository
2. Run the application with Docker Compose:
   ```
   docker-compose up -d
   ```
3. Access the Swagger UI at `http://localhost:8080/swagger`

## API Endpoints

The API provides the following endpoints:

- `GET /api/tasks`: Get all tasks
- `GET /api/tasks/{id}`: Get a specific task
- `POST /api/tasks`: Create a new task
- `PATCH /api/tasks/{id}/status`: Update a task status

## Task Lifecycle

Tasks follow a specific lifecycle:
1. **NotStarted**: Initial state when a task is created
2. **InProgress**: Task has been started but not completed
3. **Completed**: Task has been finished

Valid transitions:
- NotStarted → InProgress
- InProgress → Completed

## Message Handling

When a task is marked as completed, a message is published to the Azure Storage Queue `TaskCompleted`. A background service processes messages from this queue, currently just logging them for demonstration purposes.

## Assumptions and Limitations

- Uses Azurite for local Azure Storage emulation
- Simplified error handling for demo purposes
- No authentication/authorization implementation in this version
- No pagination implemented for the GET endpoints
- Docker setup is for development purposes only and would need hardening for production

## Future Enhancements

- Implement authentication and authorization
- Add pagination for GET endpoints
- Add more advanced filtering and sorting options
- Implement more comprehensive test coverage
- Add CI/CD pipeline for automated testing and deployment
