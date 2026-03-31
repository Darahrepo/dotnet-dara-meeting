using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface ISampleUserRepository
    {
        Task<List<SampleUser>> GetAll();
        Task<SampleUser> GetById(int id);
        Task<int> Create(SampleUser sampleUser, CancellationToken cancellationToken);
        Task<int> Update(SampleUser sampleUser, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
