using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeetingScheduler.Domain.Enums
{
    public enum WebexMeetingStatus
    {
        [Description("Successful request with body content.")]
        OK = 200,

        [Description("The request has succeeded and has led to the creation of a resource.")]
        Created = 201,

        [Description("The request was invalid or cannot be otherwise served. An accompanying error message will explain further.")]
        BadRequest = 400 ,

        [Description("Authentication credentials were missing or incorrect.")]
        Unauthorized = 401,

        [Description("Too many requests. Please retry after {Retry-After}")]
        TooManyRequests = 429,
    }
}
