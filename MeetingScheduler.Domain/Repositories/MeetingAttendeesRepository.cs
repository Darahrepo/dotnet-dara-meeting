using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.Domain.Common.Interfaces;

namespace MeetingScheduler.Domain.Repositories
{
    public class MeetingAttendeesRepository : IMeetingAttendeesRepository
    {

        private readonly IApplicationDbContext _context;

        public MeetingAttendeesRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MeetingAttendee>> GetByMeetingId(int meetingId)
        {
            return await _context.MeetingAttendees.Where(x => x.MeetingId == meetingId).ToListAsync();
        }

        //public async Task<MeetingAttendee> GetMeetingHost(int meetingId)
        //{
        //    return await _context.MeetingAttendees.Where(x => x.MeetingId == meetingId && x.IsActive == true).FirstOrDefaultAsync();
        //}

        //public async Task<int> Update(Meeting meeting, CancellationToken cancellationToken)
        //{
        //    var entity = await _context.Meetings.FindAsync(meeting.Id);

        //    if (entity == null)
        //    {
        //        throw new NotFoundException(nameof(Meeting), meeting.Id);
        //    }

        //    entity.MeetingRoom = meeting.MeetingRoom;
        //    entity.MeetingAgenda = meeting.MeetingAgenda;
        //    entity.MeetingAttendees = meeting.MeetingAttendees;
        //    entity.MeetingLink = meeting.MeetingLink;
        //    entity.MeetingLocationType = meeting.MeetingLocationType;
        //    entity.MeetingRequirements = meeting.MeetingRequirements;
        //    entity.MeetingRoom = meeting.MeetingRoom;
        //    entity.MeetingStatus = meeting.MeetingStatus;
        //    entity.Reason = meeting.Reason;
        //    entity.To = meeting.To;
        //    entity.Date = meeting.Date;
        //    entity.From = meeting.From;

        //    await _context.SaveChangesAsync(cancellationToken);

        //    return meeting.Id;
        //}


        //public async Task<int> Delete(int id, MeetingStatus status, CancellationToken cancellationToken)
        //{
        //    var entity = await _context.Meetings.FindAsync(id);

        //    if (entity == null)
        //    {
        //        throw new NotFoundException(nameof(Meeting), id);
        //    }

        //    entity.IsActive = false;
        //    entity.MeetingStatus = status;

        //    await _context.SaveChangesAsync(cancellationToken);

        //    return id;
        //}

    }
}
