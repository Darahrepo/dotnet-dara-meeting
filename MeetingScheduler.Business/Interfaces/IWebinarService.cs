using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface IWebinarService
    {
        Task<List<Webinar>> GetAll();
        Task<List<Webinar>> GetAllCeo(int userId);
        Task<List<Webinar>> GetAllForAdminApproval();
         Task<List<Webinar>> GetArchived();
        Task<List<Webinar>> GetPendingApprovals();
        Task<List<Webinar>> GetByStatus(ApprovalStatus status);
        Task<List<Webinar>> GetAllByStatus(ApprovalStatus status);
        Task<Webinar> GetById(int id);
        Task<List<Webinar>> GetAllByEmployeeId(int userId);
        Task<List<Webinar>> GetPendingByEmployeeId(int userId);
        Task<List<Webinar>> GetTodaysWebinarsByEmployeeId(int userId);
        Task<List<Webinar>> GetScheduledByEmployeeId(int userId);
        Task<List<Webinar>> GetArchivedByEmployeeId(int userId);
        Task<List<Webinar>> GetByEmployeeIdAsHost(int userId);
        Task<List<Webinar>> GetByEmployeeIdAsAttendee(int currentUserId);
        Task<int> Create(Webinar Webinar, CancellationToken cancellationToken);
        Task<int> Update(Webinar Webinar);
        Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken);
        Task<int> Cancel(int id, CancellationToken cancellationToken);
        //Task<MeetingAvailability> CheckIfWebinarTimeAvailableForHost(DateTime date, TimeSpan from, TimeSpan to, int hostId, int meetingId);
    }
}
