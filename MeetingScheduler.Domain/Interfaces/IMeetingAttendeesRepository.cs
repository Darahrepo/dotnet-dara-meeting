using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface IMeetingAttendeesRepository
    {
        Task<List<MeetingAttendee>> GetByMeetingId(int Id);
        //Task<MeetingAttendee> GetMeetingHost(int meetingId);
        //Task<List<Meeting>> GetAll();
        //Task<List<Meeting>> GetByEmployeeId(int userId)
        //Task<int> Create(Meeting meeting, CancellationToken cancellationToken);
        //Task<int> Update(Meeting meeting, CancellationToken cancellationToken);
        //Task<int> Delete(int id, MeetingStatus status, CancellationToken cancellationToken);
    }
}
