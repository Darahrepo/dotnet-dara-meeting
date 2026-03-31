using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface IWebinarRepository
    {
        Task<List<Webinar>> GetAll();
        Task<List<Webinar>> GetAllCeo(int userId);
        Task<List<Webinar>> GetArchived();
        Task<List<Webinar>> GetPendingApprovals();
        Task<Webinar> GetById(int Id);
        Task<List<Webinar>> GetAllByEmployeeId(int userId);
        Task<List<Webinar>> GetByEmployeeIdAsHost(int userId);
        Task<List<Webinar>> GetByEmployeeIdAsAttendee(int userId);
        Task<int> Create(Webinar webinar, CancellationToken cancellationToken);
        Task<int> Update(Webinar webinar, CancellationToken cancellationToken);
        Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken);
        Task<int> Cancel(int id, CancellationToken cancellationToken);

    }
}
