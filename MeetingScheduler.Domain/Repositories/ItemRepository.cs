using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.Domain.Common.Interfaces;
namespace MeetingScheduler.Domain.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IApplicationDbContext _context;


        public ItemRepository(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<Item>> GetAll()
        {
            return await _context.Items.Where(x => x.IsActive == true).ToListAsync();
        }


        public async Task<Item> GetById(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<int> Create(Item meetingItem, CancellationToken cancellationToken)
        {
            _context.Items.Add(meetingItem);
            var entity = await _context.SaveChangesAsync(cancellationToken);
            return meetingItem.Id;
        }


        public async Task<int> Update(Item meetingItem, CancellationToken cancellationToken)
        {
            var entity = await _context.Items.FindAsync(meetingItem.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(meetingItem), meetingItem.Id);
            }

            entity.NameAr = meetingItem.NameAr;
            entity.NameEn = meetingItem.NameEn;
            await _context.SaveChangesAsync(cancellationToken);

            return meetingItem.Id;
        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _context.Items.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Item), id);
            }

            entity.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }
    }
}