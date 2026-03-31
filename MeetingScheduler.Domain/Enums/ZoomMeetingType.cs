using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MeetingScheduler.Domain.Enums
{
    public enum ZoomMeetingType
    {
        InstantMeeting = 1,

        ScheduledMeeting = 2,

        NotFixedRecurringMeeting = 3,

        FixedRecurringMeeting = 8,
    }
}
