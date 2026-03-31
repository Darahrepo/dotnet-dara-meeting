using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using MeetingScheduler.Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace MeetingScheduler.Infrastructure.Services.Meetings
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMeetingAttendeesRepository _meetingAttendeesRepository;
        private readonly IUserProvider _userProvider;
        private readonly Logger _logger;
        private readonly IMapper _mapper;
        private readonly int currentUser;
        private readonly IDateTimeService _dateTime;

        public MeetingService(IMeetingRepository MeetingRepository, IDateTimeService dateTime, IMapper mapper, IMeetingRepository meetingRepository, IMeetingAttendeesRepository meetingAttendeesRepository, IUserProvider userProvider)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
            //_logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _meetingAttendeesRepository = meetingAttendeesRepository;
            _userProvider = userProvider;
            currentUser = _userProvider.CurrentUser.UserId;
            _dateTime = dateTime;
        }

        public async Task<List<Meeting>> GetAll()
        {
            List<Meeting> meetings = new List<Meeting>();
            try
            {
                meetings = await _meetingRepository.GetAll();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meetings;
        }
        public async Task<List<Meeting>> GetAllCeo(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = await _meetingRepository.GetAllCeo(userId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }


        public async Task<List<Meeting>> GetArchived()
        {
            List<Meeting> meetings = new List<Meeting>();
            try
            {
                meetings = await _meetingRepository.GetArchived();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meetings;
        }

        public async Task<List<Meeting>> GetPendingApprovals()
        {
            List<Meeting> meetings = new List<Meeting>();
            try
            {
                meetings = await _meetingRepository.GetPendingApprovals();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meetings;
        }

        public async Task<Meeting> GetById(int id)
        {
            Meeting meeting = new Meeting();
            try
            {
               meeting = await _meetingRepository.GetById(id);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetAllByEmployeeId(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = await _meetingRepository.GetAllByEmployeeId(userId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetByEmployeeIdAsHost(int currentUserId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = await _meetingRepository.GetByEmployeeIdAsHost(currentUserId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }
        public async Task<List<Meeting>> GetByEmployeeIdAsAttendee(int currentUserId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = await _meetingRepository.GetByEmployeeIdAsAttendee(currentUserId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetPendingByEmployeeId(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = (await _meetingRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Pending && (x.Date > DateTime.Now.Date || (x.Date == DateTime.Now.Date && x.Time_From >= DateTime.Now.TimeOfDay))).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetScheduledByEmployeeId(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = (await _meetingRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && (x.Date+x.Time_To) > DateTime.Now).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetTodaysByEmployeeId(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = (await _meetingRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date == DateTime.Now.Date).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }

        public async Task<List<Meeting>> GetArchivedByEmployeeId(int userId)
        {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                meeting = (await _meetingRepository.GetAllByEmployeeId(userId)).Where(x => x.IsActive == true && ((x.ApprovalStatus != ApprovalStatus.Pending  && x.Date + x.Time_From < DateTime.Now)||x.ApprovalStatus == ApprovalStatus.Cancelled)).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }


        public async Task<List<Meeting>> GetByStatus(ApprovalStatus status) {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                var entity = (await _meetingRepository.GetByEmployeeIdAsHost(currentUser)).Where(x=>x.ApprovalStatus == status).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }
        public async Task<List<Meeting>> GetAllByStatus(ApprovalStatus status) {
            List<Meeting> meeting = new List<Meeting>();
            try
            {
                var entity = (await _meetingRepository.GetAll()).Where(x => x.ApprovalStatus == status).ToList();            
            }
            catch (Exception Ex)
            {
                throw;
            }

            return meeting;
        }


        public async Task<int> Create(Meeting meeting, CancellationToken cancellationToken)
        {
            int result = 0;
            try
            {
                result =  await _meetingRepository.Create(meeting, cancellationToken);
            }
            catch (Exception Ex)
            {
                _logger.Error(Ex.Message);
                throw;
            }

            return result;
        }

        public async Task<int> Update(Meeting meeting)
        {
            var result = 0;
            try
            {
                CancellationToken cancellation = new CancellationToken();
                result = await _meetingRepository.Update(meeting, cancellation);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return result;
        }

        public async Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken)
        {
            return await _meetingRepository.Delete(id, status, cancellationToken);
        }


        public async Task<int> Cancel(int id, CancellationToken cancellationToken)
        {
            return await _meetingRepository.Cancel(id, cancellationToken);
        }
    }
}
