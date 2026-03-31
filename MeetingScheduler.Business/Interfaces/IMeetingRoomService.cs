using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Services.MeetingRooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface IMeetingRoomService
    {
        Task<List<MeetingRoom>> GetAll();
        Task<MeetingRoom> GetById(int id);
        Task<int> Create(MeetingRoom sampleUser, CancellationToken cancellationToken);
        Task<int> Update(MeetingRoom sampleUser, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
