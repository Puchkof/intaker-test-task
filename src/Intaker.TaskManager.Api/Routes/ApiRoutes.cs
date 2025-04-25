namespace Intaker.TaskManager.Api.Routes
{
    public static class ApiRoutes
    {
        private const string ApiBase = "/api";
        
        public static class Tasks
        {
            public const string Base = $"{ApiBase}/tasks";
            public const string GetAll = Base;
            public const string Get = $"{Base}/{{id:int}}";
            public const string Create = Base;
            public const string UpdateStatus = $"{Base}/{{id:int}}/status";
        }
    }
} 