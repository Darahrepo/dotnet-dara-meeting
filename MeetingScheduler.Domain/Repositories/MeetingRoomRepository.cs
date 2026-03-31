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
    public class MeetingRoomRepository : IMeetingRoomRepository
    {
        private readonly IApplicationDbContext _context;


        public MeetingRoomRepository(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<MeetingRoom>> GetAll()
        {
            return await _context.MeetingRooms.Where(x => x.IsActive == true).ToListAsync();
        }


        public async Task<MeetingRoom> GetById(int id)
        {
            return await _context.MeetingRooms.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<int> Create(MeetingRoom MeetingRoom, CancellationToken cancellationToken)
        {
            _context.MeetingRooms.Add(MeetingRoom);
            var entity = await _context.SaveChangesAsync(cancellationToken);
            return MeetingRoom.Id;
        }


        public async Task<int> Update(MeetingRoom MeetingRoom, CancellationToken cancellationToken)
        {
            var entity = await _context.MeetingRooms.FindAsync(MeetingRoom.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(MeetingRoom), MeetingRoom.Id);
            }

            entity.NameAr = MeetingRoom.NameAr;
            entity.NameEn = MeetingRoom.NameEn;
            await _context.SaveChangesAsync(cancellationToken);

            return MeetingRoom.Id;
        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _context.MeetingRooms.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(MeetingRoom), id);
            }

            entity.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }
    }
}