using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Infrastructure.Services.Meetings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface IMeetingService
    {
        Task<List<Meeting>> GetAll();
        Task<List<Meeting>> GetAllCeo( int userId);
        Task<List<Meeting>> GetArchived();
        Task<List<Meeting>> GetPendingApprovals();
        Task<List<Meeting>> GetByStatus(ApprovalStatus status);
        Task<List<Meeting>> GetAllByStatus(ApprovalStatus status);
        Task<Meeting> GetById(int id);
        Task<List<Meeting>> GetAllByEmployeeId(int userId);
        Task<List<Meeting>> GetPendingByEmployeeId(int userId);
        Task<List<Meeting>> GetScheduledByEmployeeId(int userId);
        Task<List<Meeting>> GetArchivedByEmployeeId(int userId);
        Task<List<Meeting>> GetTodaysByEmployeeId(int userId);
        Task<List<Meeting>> GetByEmployeeIdAsHost(int userId);
        Task<List<Meeting>> GetByEmployeeIdAsAttendee(int currentUserId);
        Task<int> Create(Meeting meeting, CancellationToken cancellationToken);
        Task<int> Update(Meeting meeting );
        Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken);
        Task<int> Cancel(int id, CancellationToken cancellationToken);
    }
}
