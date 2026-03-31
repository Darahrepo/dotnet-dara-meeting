using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IZoomService
    {
        Task<MeetingDetails> CreateMeeting(MeetingDetails details);
        Task<ZoomWebinarDetails> CreateWebinar(ZoomWebinarDetails details);
        Task<int> CancelMeeting(string zoomMeetingId, ZoomUserType zoomAccount =ZoomUserType.Main);
        Task<int> CancelWebinar(string zoomWebinarId, ZoomUserType zoomAccount =ZoomUserType.Main);
        Task<List<ZoomMeetingDetails>> GetUpcomingMeetings(DateTime from ,ZoomUserType zoomAccount =ZoomUserType.Main);
        Task<List<ZoomMeetingDetails>> GetUpcomingWebinars(DateTime from,ZoomUserType zoomAccount =ZoomUserType.Main);
    }
}
