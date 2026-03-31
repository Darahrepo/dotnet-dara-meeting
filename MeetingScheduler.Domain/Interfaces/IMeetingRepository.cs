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
    public interface IMeetingRepository
    {
        Task<List<Meeting>> GetAll();
        Task<List<Meeting>> GetAllCeo(int userId);
        Task<List<Meeting>> GetArchived();
        Task<List<Meeting>> GetPendingApprovals();
        Task<Meeting> GetById(int Id);
        Task<List<Meeting>> GetAllByEmployeeId(int userId);
        Task<List<Meeting>> GetByEmployeeIdAsHost(int userId);
        Task<List<Meeting>> GetByEmployeeIdAsAttendee(int userId);
        Task<int> Create(Meeting meeting, CancellationToken cancellationToken);
        Task<int> Update(Meeting meeting, CancellationToken cancellationToken);
        Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken);
        Task<int> Cancel(int id, CancellationToken cancellationToken);

    }
}
