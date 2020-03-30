namespace TweetBook.Contracts.V1
{
    public static partial class ApiRoutes
    {
        public const string ControllerRoute = "api/v1";

        public static class Posts
        {
            public const string GetAll = "posts";
            public const string Get = "posts/{postId}";
            public const string Create = "post";
        }
    }
}
