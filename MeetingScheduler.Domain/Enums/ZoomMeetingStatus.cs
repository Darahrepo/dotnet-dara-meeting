using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeetingScheduler.Domain.Enums
{
    public enum ZoomMeetingStatus
    {
        [Description("Meeting created.")]
        success = 201 ,

        [Description("A maximum of {rateLimitNumber} meetings can be created/updated for a single user in one day.")]
        MaximumMeetingsCreated= 300 ,

        [Description(" Bad Request.")]
        BadRequest = 400 ,

        [Description(" Bad Request.")]
        NotFound = 1001 ,

        [Description("User {userId} not exist or not belong to this account. ")]
        NotFound2 = 1001 ,
    }
}
