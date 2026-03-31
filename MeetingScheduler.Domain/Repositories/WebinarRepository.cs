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
    public class WebinarRepository : IWebinarRepository
    {

        private readonly IApplicationDbContext _context;
        private readonly Logger _logger;
        private readonly IDateTimeService _dateTimeService;

        public WebinarRepository(ILogger<WebinarRepository> logger, IApplicationDbContext context, IDateTimeService dateTimeService)
        {
            _context = context;
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _dateTimeService = dateTimeService;
        }

        public async Task<List<Webinar>> GetAll()
        {
            return await _context.Webinars.Where(x => x.IsActive == true)
                        .Include(x => x.Host)
                        .Include(x => x.Interpreters)
                        .Include(x => x.WebinarPanelists.Where(x => x.IsActive == true))
                        .Include(x => x.WebinarRequirements.Where(x => x.IsActive == true))
                        .Include(x => x.WebinarRequirements.Where(x => x.IsActive == true))
                        .ToListAsync();
        }
        public async Task<List<Webinar>> GetAllCeo(int userId)
        {
            return await _context.Webinars.Where(x => ((x.WebinarPanelists.Any(x => x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved) || x.HostId == userId || x.ZoomAccount == ZoomUserType.Ceo) && x.IsActive == true)
                        .Include(x => x.Host)
                        .Include(x => x.Interpreters)
                        .Include(x => x.WebinarPanelists.Where(x => x.IsActive == true))
                        .ThenInclude(x => x.Employee)
                        .ToListAsync();
        }
        public async Task<List<Webinar>> GetArchived()
        {
            return await _context.Webinars.Where(x => x.IsActive == true && x.ApprovalStatus != ApprovalStatus.Pending && x.Date < DateTime.Now.Date)
                        .Include(x => x.Host)
                        .OrderByDescending(x => x.CreatedOn)
                            .ThenByDescending(x => x.Time_From)
                        .ToListAsync();
        }

        public async Task<List<Webinar>> GetPendingApprovals()
        {
            return await _context.Webinars.Where(x => x.IsActive == true && x.ApprovalStatus == ApprovalStatus.Pending 
                            && (x.Date > _dateTimeService.Now.Date || (x.Date == _dateTimeService.Now.Date && x.Time_From >= _dateTimeService.Now.TimeOfDay)))
                        .Include(x => x.Host)
                        .Include(x => x.Interpreters)
                        .OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From)
                        .ToListAsync();
        }

        public async Task<List<Webinar>> GetAllByEmployeeId(int userId)
        {
            return await _context.Webinars.Where(x => ((x.WebinarPanelists.Any(x => x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved )|| x.HostId == userId ) && x.IsActive == true )
                        .Include(x => x.Host)
                        .Include(x => x.Interpreters)
                        .Include(x => x.WebinarPanelists.Where(x => x.IsActive == true))
                        .ThenInclude(x => x.Employee)
                        .ToListAsync();
        } 

        public async Task<List<Webinar>> GetAllDuringTimeSpan(DateTime Date, TimeSpan From , TimeSpan To)
        {
            return await _context.Webinars.Where(x => x.IsActive == true ).ToListAsync();
        }


        public async Task<Webinar> GetById(int id)
        {
            return await _context.Webinars.Where(x=>x.IsActive==true)
                        .Include(x => x.Interpreters)
                            .ThenInclude(x=>x.FromLanguage)
                        .Include(x => x.Interpreters)
                            .ThenInclude(x => x.ToLanguage)
                        .Include(x=>x.WebinarPanelists.Where(x=>x.IsActive==true))
                        .ThenInclude(x=>x.Employee)
                        .Include(x=>x.WebinarRequirements.Where(x => x.IsActive == true))
                        .Include(x=>x.Host)
                        .Include(x=>x.WebinarAttachments)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Webinar>> GetByEmployeeIdAsHost(int userId)
        {
            var requests = await _context.Webinars.Where(x => x.HostId == userId && x.IsActive == true)
                            .Include(x => x.Host)
                            .Include(x => x.Interpreters)
                            .Include(x => x.WebinarPanelists.Where(x => x.IsActive == true))
                            .ThenInclude(x => x.Employee)
                            .ToListAsync();
            return requests;
        }

        public async Task<List<Webinar>> GetByEmployeeIdAsAttendee(int userId)
        {
            var requests = await _context.Webinars.Where(x => x.WebinarPanelists.Any(x=>x.EmployeeId == userId) && x.ApprovalStatus == ApprovalStatus.Approved && x.IsActive == true)
                            .Include(x => x.Host)
                            .Include(x => x.Interpreters)
                            .Include(x => x.WebinarPanelists.Where(x => x.IsActive == true))
                            .ThenInclude(x => x.Employee)
                            .ToListAsync();
            return requests;
        }

        public async Task<int> Create(Webinar webinar, CancellationToken cancellationToken)
        {
            var entity = 0;
            try
            {
                _context.Webinars.Add(webinar);
                entity = await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return webinar.Id;
        }


        public async Task<int> Update(Webinar webinar, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Webinars.FindAsync(webinar.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Webinar), webinar.Id);
                }

                entity.HostId = webinar.HostId;
                entity.Agenda = webinar.Agenda;
                entity.ZoomWebinarPassword = webinar.ZoomWebinarPassword;
                entity.WebinarPanelists = webinar.WebinarPanelists;
                entity.WebinarUrl = webinar.WebinarUrl;
                entity.WebinarRequirements = webinar.WebinarRequirements;
                entity.ApprovalStatus = webinar.ApprovalStatus;
                entity.Reason = webinar.Reason;
                entity.Time_To = webinar.Time_To;
                entity.Date = webinar.Date;
                entity.Time_From = webinar.Time_From;
                entity.WebinarAttachments = webinar.WebinarAttachments;

                var result = await _context.SaveChangesAsync(cancellationToken);
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
            return webinar.Id;
        }


        public async Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken)
        {
            var entity = await _context.Webinars.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Webinar), id);
            }

            entity.IsActive = false;
            entity.ApprovalStatus = status;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }

        public async Task<int> Cancel(int id,  CancellationToken cancellationToken)
        {
            var entity = await _context.Webinars.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Webinar), id);
            }

            entity.ApprovalStatus = ApprovalStatus.Cancelled;

            await _context.SaveChangesAsync(cancellationToken);

            return id;
        }

    }
}
