namespace PersonalWebsiteBackend.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Projects
        {
            public const string GetAll = Base + "/projects";

            public const string Get = Base + "/projects/{projectId}";

            public const string Update = Base + "/projects/{projectId}";

            public const string Delete = Base + "/projects/{projectId}";

            public const string Create = Base + "/projects";
        }
        
        public static class Documents
        {
            public const string GetAll = Base + "/documents";

            public const string Get = Base + "/documents/{documentId}";

            public const string Update = Base + "/documents/{documentId}";

            public const string Delete = Base + "/documents/{documentId}";

            public const string Create = Base + "/documents";
        }
        
        

        public static class Tags
        {
            public const string GetAll = Base + "/tags";

            public const string Get = Base + "/tags/{tagName}";

            public const string Create = Base + "/tags";

            public const string Delete = Base + "/tags/{tagName}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";

            public const string Register = Base + "/identity/register";

            public const string Refresh = Base + "/identity/refresh";
        }
    }
}