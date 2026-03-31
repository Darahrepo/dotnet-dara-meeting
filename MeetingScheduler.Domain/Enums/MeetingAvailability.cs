using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Enums
{
    public enum MeetingAvailability
    {
        HostHasConflictingMeeting,
        HostHasConflictingWebinar,
        HostHasConflictingZoomMeeting,
        HostHasConflictingZoomWebinar,
        HostHasConflictingWebexMeeting,
        HostHasConflictingWebexWebinar,
        RoomNotAvailable,
        TimingNotAvailable,
        IssueAuthorizingUser,
        Available
    }
}