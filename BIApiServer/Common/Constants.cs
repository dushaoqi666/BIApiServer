public static class Constants
{
    public static class CacheExpiration
    {
        public static readonly TimeSpan ShortTerm = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan MediumTerm = TimeSpan.FromHours(1);
        public static readonly TimeSpan LongTerm = TimeSpan.FromDays(1);
    }

    public static class ErrorCodes
    {
        public const int Success = 200;
        public const int ValidationError = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int ServerError = 500;
    }
} 