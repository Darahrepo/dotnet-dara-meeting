using Microsoft.AspNetCore.Builder;


namespace MeetingScheduler.Identity
{ 
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAdMiddleware(this IApplicationBuilder builder) =>
            builder.UseMiddleware<AdUserMiddleware>();
    }
}