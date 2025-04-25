FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Intaker.TaskManager.Api/Intaker.TaskManager.Api.csproj", "src/Intaker.TaskManager.Api/"]
COPY ["src/Intaker.TaskManager.Application/Intaker.TaskManager.Application.csproj", "src/Intaker.TaskManager.Application/"]
COPY ["src/Intaker.TaskManager.Domain/Intaker.TaskManager.Domain.csproj", "src/Intaker.TaskManager.Domain/"]
COPY ["src/Intaker.TaskManager.Infrastructure/Intaker.TaskManager.Infrastructure.csproj", "src/Intaker.TaskManager.Infrastructure/"]
RUN dotnet restore "src/Intaker.TaskManager.Api/Intaker.TaskManager.Api.csproj"
COPY . .
WORKDIR "/src/src/Intaker.TaskManager.Api"
RUN dotnet build "Intaker.TaskManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Intaker.TaskManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Intaker.TaskManager.Api.dll"] 