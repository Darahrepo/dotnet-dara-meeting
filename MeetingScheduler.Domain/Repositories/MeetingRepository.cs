using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {

        private readonly IApplicationDbContext _context;
        private readonly Logger _logger;
        private readonly IDateTimeService _dateTimeService;

        public MeetingRepository(ILogger<MeetingRepository> logger, IApplicationDbContext context, IDateTimeService dateTimeService)
        {
            _context = context;
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _dateTimeService = dateTimeService;
        }
        public async Task<List<Meeting>> GetAllCeo(int userId)
        {
            return await _context.Meetings.Where(x => ((x.MeetingAttendees.Any(x => x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved) || x.HostId == userId || x.ZoomAccount == ZoomUserType.Ceo) && x.IsActive == true)
                        .Include(x => x.Host)
                        .Include(x => x.MeetingRoom)
                        .Include(x => x.MeetingAttendees.Where(x => x.IsActive == true))
                        .ThenInclude(x => x.Employee)
                        .ToListAsync();
        }
        public async Task<List<Meeting>> GetAll()
        {
            return await _context.Meetings.Where(x => x.IsActive == true)
                        .Include(x => x.Host)
                        .Include(x => x.MeetingAttendees.Where(x => x.IsActive == true))
                        .Include(x => x.MeetingRequirements.Where(x => x.IsActive == true))
                        .Include(x => x.MeetingRoom)
                        .ToListAsync();
        }

        public async Task<List<Meeting>> GetArchived()
        {
            return await _context.Meetings.Where(x => x.IsActive == true && ((x.ApprovalStatus != ApprovalStatus.Pending && (x.Date <= DateTime.Now.Date && x.Time_To <= DateTime.Now.TimeOfDay)) || x.ApprovalStatus == ApprovalStatus.Cancelled))
                        .Include(x => x.Host)
                        .Include(x => x.MeetingRoom)
                        .OrderByDescending(x => x.Date)
                            .ThenByDescending(x => x.Time_From)
                        .ToListAsync();
        }

        public async Task<List<Meeting>> GetPendingApprovals()
        {
            return await _context.Meetings.Where(x => x.IsActive == true && x.ApprovalStatus == ApprovalStatus.Pending 
                            && (x.Date > _dateTimeService.Now.Date || (x.Date == _dateTimeService.Now.Date && x.Time_From >= _dateTimeService.Now.TimeOfDay)))
                        .Include(x => x.Host)
                        .Include(x => x.MeetingRoom)
                        .OrderByDescending(x => x.Date)
                            .ThenByDescending(x => x.Time_From)
                        .ToListAsync();
        }

        public async Task<List<Meeting>> GetAllByEmployeeId(int userId)
        {
            return await _context.Meetings.Where(x => ((x.MeetingAttendees.Any(x => x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved )|| x.HostId == userId ) && x.IsActive == true)
                        .Include(x => x.Host)
                        .Include(x => x.MeetingRoom)
                        .Include(x => x.MeetingAttendees.Where(x => x.IsActive == true))
                        .ThenInclude(x => x.Employee)
                        .ToListAsync();
        } 

        public async Task<List<Meeting>> GetAllDuringTimeSpan(DateTime Date, TimeSpan From , TimeSpan To)
        {
            return await _context.Meetings.Where(x => x.IsActive == true ).ToListAsync();
        }


        public async Task<Meeting> GetById(int id)
        {
            return await _context.Meetings.Where(x=>x.IsActive==true)
                        .Include(x=>x.MeetingAttendees.Where(x=>x.IsActive==true))
                        .ThenInclude(x=>x.Employee)
                        .Include(x=>x.MeetingRequirements.Where(x => x.IsActive == true))
                        .Include(x=>x.Host)
                        .Include(x=>x.MeetingRoom)
                        .Include(x=>x.MeetingAttachments)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Meeting>> GetByEmployeeIdAsHost(int userId)
        {
            var requests = await _context.Meetings.Where(x => x.HostId == userId && x.IsActive == true)
                            .Include(x => x.Host)
                            .Include(x => x.MeetingRoom)
                            .Include(x => x.MeetingAttendees.Where(x => x.IsActive == true))
                            .ThenInclude(x => x.Employee)
                            .ToListAsync();
            return requests;
        }

        public async Task<List<Meeting>> GetByEmployeeIdAsAttendee(int userId)
        {
            var requests = await _context.Meetings.Where(x => x.MeetingAttendees.Any(x=>x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved && x.IsActive == true)
                            .Include(x => x.Host)
                            .Include(x => x.MeetingRoom)
                            .Include(x => x.MeetingAttendees.Where(x => x.IsActive == true))
                            .ThenInclude(x => x.Employee)
                            .ToListAsync();
            return requests;
        }

        public async Task<int> Create(Meeting meeting, CancellationToken cancellationToken)
        {
            var entity = 0;
            try
            {
                _context.Meetings.Add(meeting);
                entity = await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return meeting.Id;
        }


        public async Task<int> Update(Meeting meeting, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Meetings.FindAsync(meeting.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Meeting), meeting.Id);
                }

                entity.MeetingRoomId = meeting.MeetingRoomId;
                entity.HostId = meeting.HostId;
                entity.MeetingAgenda = meeting.MeetingAgenda;
                entity.MeetingAttendees = meeting.MeetingAttendees;
                entity.MeetingLink = meeting.MeetingLink;
                entity.MeetingLocationType = meeting.MeetingLocationType;
                entity.MeetingRequirements = meeting.MeetingRequirements;
                entity.MeetingRoomId = meeting.MeetingRoomId;
                entity.ApprovalStatus = meeting.ApprovalStatus;
                entity.Reason = meeting.Reason;
                entity.Time_To = meeting.Time_To;
                entity.Date = meeting.Date;
                entity.Time_From = meeting.Time_From;
                entity.MeetingAttendees = meeting.MeetingAttendees;
                entity.MeetingAttachments = meeting.MeetingAttachments;

                var result = await _context.SaveChangesAsync(cancellationToken);
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
            return meeting.Id;
        }


        public async Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken)
        {
            var entity = await _context.Meetings.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Meeting), id);
            }

            entity.IsActive = false;
            entity.ApprovalStatus = status;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }

        public async Task<int> Cancel(int id,  CancellationToken cancellationToken)
        {
            var entity = await _context.Meetings.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Meeting), id);
            }

            entity.ApprovalStatus = ApprovalStatus.Cancelled;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }

    }
}
