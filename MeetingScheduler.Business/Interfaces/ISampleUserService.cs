using MeetingScheduler.Infrastructure.Services.SampleUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface ISampleUserService
    {
        Task<List<SampleUserDto>> GetAll();
        Task<SampleUserDto> GetById(int id);
        Task<int> Create(SampleUserDto sampleUser, CancellationToken cancellationToken);
        Task<int> Update(SampleUserDto sampleUser, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
