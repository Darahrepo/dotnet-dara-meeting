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
    public class SampleUserRepository : ISampleUserRepository
    {
        
        
        private readonly IApplicationDbContext _context;


        public SampleUserRepository(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<SampleUser>> GetAll()
        {
            return await _context.SampleUsers.Where(x=>x.IsActive ==true).ToListAsync();
        }


        public async Task<SampleUser> GetById(int id) 
        {
            return await _context.SampleUsers.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<int> Create(SampleUser sampleUser, CancellationToken cancellationToken) 
        { 
            _context.SampleUsers.Add(sampleUser);
            var entity = await _context.SaveChangesAsync(cancellationToken);
            return sampleUser.Id;
        }


        public async Task<int> Update(SampleUser sampleUser, CancellationToken cancellationToken) 
        {
            var entity = await _context.SampleUsers.FindAsync(sampleUser.Id);

            if(entity == null)
            {
                throw new NotFoundException(nameof(SampleUser), sampleUser.Id);
            }
            entity.NameAr = sampleUser.NameAr;
            entity.NameEn = sampleUser.NameEn;
            entity.EmailAddress = sampleUser.EmailAddress;
            await _context.SaveChangesAsync(cancellationToken);
            return sampleUser.Id;
        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _context.SampleUsers.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(SampleUser), id);
            }

            entity.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }

    }
}
