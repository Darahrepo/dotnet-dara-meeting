using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class MeetingValidationService: IMeetingValidationService
    {
        public readonly IMeetingRepository _meetingRepository; 
        public readonly IWebinarRepository _webinarRepository;
        private readonly IDateTimeService _dateTime;
        private readonly IZoomService _zoomService;
        private readonly IWebexService _webexService;

        public MeetingValidationService(IMeetingRepository meetingRepository, IWebinarRepository webinarRepository, IWebexService webexService, IDateTimeService dateTime, IZoomService zoomService)
        {
            _meetingRepository = meetingRepository;
            _webinarRepository = webinarRepository;
            _dateTime = dateTime;
            _zoomService = zoomService;
            _webexService = webexService;
        }

        public async Task<MeetingAvailability> CheckIfTimeAvailableForHost(MeetingValidity meetingValidity, ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            bool result = false;
            try
            {

				//check existing meetings in db for room availability and host availability
				var meetingCanBeReserved = (await _meetingRepository.GetAll())
                                            .Where(x => x.Id != meetingValidity.MeetingId
                                            && x.Date >= _dateTime.Now.Date
                                            && x.ZoomAccount == zoomAccount
                                            && (x.ApprovalStatus == ApprovalStatus.Approved || x.ApprovalStatus == ApprovalStatus.Pending)
                                            && x.IsActive == true
                                            && ((((x.Date + x.Time_From >= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_From <= meetingValidity.Date + meetingValidity.To)
                                            || (x.Date + x.Time_To >= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_To <= meetingValidity.Date + meetingValidity.To)
                                            || (x.Date + x.Time_From <= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_To >= meetingValidity.Date + meetingValidity.To))))).ToList();

                var roomNotAvailable = meetingCanBeReserved.Any(x => x.MeetingRoomId == meetingValidity.RoomId);

                if (roomNotAvailable)
                {
                    return MeetingAvailability.RoomNotAvailable;
                }

                var hostNotAvailable_meet = meetingCanBeReserved.Any(x => x.Date == meetingValidity.Date && (x.HostId == meetingValidity.HostId || x.MeetingAttendees.Any(e => e.EmployeeId == meetingValidity.HostId)));

                if (hostNotAvailable_meet)
                {
                    return MeetingAvailability.HostHasConflictingMeeting;
                }

                var webinarCanBeReserved = (await _webinarRepository.GetAll())
                                            .Where(x => x.Id != meetingValidity.MeetingId
                                            && x.Date >= _dateTime.Now.Date
                                            && x.ZoomAccount == zoomAccount
                                            && (x.ApprovalStatus == ApprovalStatus.Approved || x.ApprovalStatus == ApprovalStatus.Pending)
                                            && x.IsActive == true
                                            && ((((x.Date + x.Time_From >= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_From <= meetingValidity.Date + meetingValidity.To)
                                            || (x.Date + x.Time_To >= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_To <= meetingValidity.Date + meetingValidity.To)
                                            || (x.Date + x.Time_From <= meetingValidity.Date + meetingValidity.From && x.Date + x.Time_To >= meetingValidity.Date + meetingValidity.To))))).ToList();

                var hostNotAvailable_web = webinarCanBeReserved.Any(
                                            x => x.Id != meetingValidity.WebinarId
                                            && (x.HostId == meetingValidity.HostId || x.WebinarPanelists.Any(e => e.EmployeeId == meetingValidity.HostId)));

                if (hostNotAvailable_web)
                {
                    return MeetingAvailability.HostHasConflictingWebinar;
                }

                MeetingAvailability scheduledMeetings;
                if (meetingValidity.IsWebex)
                {
                     scheduledMeetings = await getScheduledWebexMeetings(meetingValidity,(WebexUserType) zoomAccount);
                }
                else
                {
                     scheduledMeetings = await getScheduledZoomMeetings(meetingValidity, zoomAccount);
                }
                if (scheduledMeetings != MeetingAvailability.Available)
                {
                    return scheduledMeetings;
                }

                var timingNotAvailable = false;
                if (meetingValidity.LocationType == (char)LocationType.Both || meetingValidity.LocationType == (char)LocationType.Online)
                {
                    timingNotAvailable = meetingCanBeReserved.Any(x => x.MeetingLink != null) || webinarCanBeReserved.Any();
                }

                if (timingNotAvailable)
                {
                    return MeetingAvailability.TimingNotAvailable;
                }

            }
            catch (Exception Ex)
            {
                throw;
            }

            return MeetingAvailability.Available;
        }


        private async Task<MeetingAvailability> getScheduledZoomMeetings( MeetingValidity meetingValidity, ZoomUserType zoomAccount)
        {
            var meetings = await _zoomService.GetUpcomingMeetings(meetingValidity.Date + meetingValidity.From, zoomAccount);

            if (meetings == null)
            {
                return MeetingAvailability.IssueAuthorizingUser;
            }

            var meetingExists = meetings.Any(
                                       x =>
                                       ((((x.start_time >= meetingValidity.Date + meetingValidity.From && x.start_time <= meetingValidity.Date + meetingValidity.To)
                                       || (x.start_time.AddMinutes(x.duration) >= meetingValidity.Date + meetingValidity.From && x.start_time.AddMinutes(x.duration) <= meetingValidity.Date + meetingValidity.To)
                                       || (x.start_time <= meetingValidity.Date + meetingValidity.From && x.start_time.AddMinutes(x.duration) >= meetingValidity.Date + meetingValidity.To)
                                       ))));
            if (meetingExists)
            {
                return MeetingAvailability.HostHasConflictingZoomMeeting;
            }
            var webinars = await _zoomService.GetUpcomingWebinars(meetingValidity.Date + meetingValidity.From, zoomAccount);

            if (webinars == null)
            {
                return MeetingAvailability.IssueAuthorizingUser;
            }

            var webinarsExist = meetings.Any(
                                       x =>
                                       ((((x.start_time >= meetingValidity.Date + meetingValidity.From && x.start_time <= meetingValidity.Date + meetingValidity.To)
                                       || (x.start_time.AddMinutes(x.duration) >= meetingValidity.Date + meetingValidity.From && x.start_time.AddMinutes(x.duration) <= meetingValidity.Date + meetingValidity.To)
                                       || (x.start_time <= meetingValidity.Date + meetingValidity.From && x.start_time.AddMinutes(x.duration) >= meetingValidity.Date + meetingValidity.To)
                                       ))));

            if (webinarsExist)
            {
                return MeetingAvailability.HostHasConflictingZoomWebinar;
            }


            return MeetingAvailability.Available;
        }

        private async Task<MeetingAvailability> getScheduledWebexMeetings(MeetingValidity meetingValidity, WebexUserType webexAccount)
        {
            var meetings = new List<MeetingDetails>();

            try
            {
                meetings = await _webexService.GetUpcomingMeetings(meetingValidity.Date + meetingValidity.From, webexAccount);
                if (meetings == null)
                {
                    return MeetingAvailability.IssueAuthorizingUser;
                }

                var meetingExists = meetings.Any(
                                           x =>
                                           ((((x.StartDateTime.ToLocalTime() >= meetingValidity.Date + meetingValidity.From && x.StartDateTime.ToLocalTime() <= meetingValidity.Date + meetingValidity.To)
                                           || (x.EndDateTime.ToLocalTime() >= meetingValidity.Date + meetingValidity.From && x.EndDateTime.ToLocalTime() <= meetingValidity.Date + meetingValidity.To)
                                           || (x.StartDateTime.ToLocalTime() <= meetingValidity.Date + meetingValidity.From && x.EndDateTime.ToLocalTime() >= meetingValidity.Date + meetingValidity.To)
                                           ))));
                if (meetingExists)
                {
                    return MeetingAvailability.HostHasConflictingMeeting;
                }
                //var webinars = await _webexService.GetUpcomingWebinars(meetingValidity.Date + meetingValidity.From, webexAccount);

                //if (webinars == null)
                //{
                //    return MeetingAvailability.IssueAuthorizingUser;
                //}

                //var webinarsExist = meetings.Any(
                //                           x =>
                //                           ((((x.StartDateTime >= meetingValidity.Date + meetingValidity.From && x.StartDateTime <= meetingValidity.Date + meetingValidity.To)
                //                           || (x.EndDateTime  >= meetingValidity.Date + meetingValidity.From && x.EndDateTime <= meetingValidity.Date + meetingValidity.To)
                //                           || (x.StartDateTime <= meetingValidity.Date + meetingValidity.From && x.EndDateTime >= meetingValidity.Date + meetingValidity.To)
                //                           ))));

                //if (webinarsExist)
                //{
                //    return MeetingAvailability.HostHasConflictingWebinar;
                //}

            }
            catch(Exception ex)
            {
                throw ex;
            }


            return MeetingAvailability.Available;
        }
    }
}
