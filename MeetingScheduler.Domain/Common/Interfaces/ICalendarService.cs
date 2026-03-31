using Ical.Net;
using MeetingScheduler.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface ICalendarService
    {
        Task<string> CreateICS(MeetingDetails details);
    }
}
