using MeetingScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface IItemRepository
    {
        Task<List<Item>> GetAll();
        Task<Item> GetById(int id);
        Task<int> Create(Item item, CancellationToken cancellationToken);
        Task<int> Update(Item item, CancellationToken cancellationToken);
        Task<int> Delete(int id, CancellationToken cancellationToken);
    }
}
