using MeetingScheduler.Infrastructure.Services.MeetingItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetAll();
        Task<ItemDto> GetById(int id);
        Task<int> Create(ItemDto sampleUser, CancellationToken cancellationToken);
        Task<int> Update(ItemDto sampleUser, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
