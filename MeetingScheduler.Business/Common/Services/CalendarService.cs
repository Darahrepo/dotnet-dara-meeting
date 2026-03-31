using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using System;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class CalendarService : ICalendarService
    {

        public async Task<string> CreateICS(MeetingDetails details)
        {
            var startDate = details.StartDateTime;

            var endDate = startDate.AddHours(details.DurationInMinutes/60);

            string description = $"DARAH Meetings is inviting you to a scheduled Zoom meeting.\r\n\r\nTopic:{(details.Topic)} \r\n\r\nJoin Zoom Meeting\r\n{details.JoiningUrl} \r\n\r\nMeeting ID: {details.MeetingId} \r\nPasscode: {details.Password} \r\n\r\n";
            
            var calendarEvent = new CalendarEvent
            {
                Description = description,
                Start = new CalDateTime(startDate, details.Timezone),
                End = new CalDateTime(endDate, details.Timezone),
                Location = details.Location,
                Summary = "Meeting Invitation to " + details.Topic,
            };

            var calendar = new Calendar();
            calendar.Events.Add(calendarEvent);
            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }

    }
}
