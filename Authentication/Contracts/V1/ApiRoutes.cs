namespace AuthenticationService.Contracts.V1
{
    public static partial class ApiRoutes
    {
        public const string ControllerRoute = "api/v1";

        public static class Identity
        {
            public const string Register = "Register";
            public const string Login = "Login";
            public const string RefreshToken = "RefreshToken";
        }
    }
}
