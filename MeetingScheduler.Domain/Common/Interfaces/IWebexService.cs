using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IWebexService
    {
        Task<MeetingDetails> CreateMeeting(MeetingDetails details);
        Task<int> CancelMeeting(string webexMeetingId, WebexUserType webexAccount);
        Task<List<MeetingDetails>> GetUpcomingMeetings(DateTime from , WebexUserType webexAccount);
	}
}
