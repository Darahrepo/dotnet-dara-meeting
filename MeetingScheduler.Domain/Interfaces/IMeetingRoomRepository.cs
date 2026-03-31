using MeetingScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface IMeetingRoomRepository
    {
        Task<List<MeetingRoom>> GetAll();
        Task<MeetingRoom> GetById(int id);
        Task<int> Create(MeetingRoom room, CancellationToken cancellationToken);
        Task<int> Update(MeetingRoom room, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
