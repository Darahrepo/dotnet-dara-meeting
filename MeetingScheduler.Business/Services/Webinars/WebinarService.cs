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

namespace MeetingScheduler.Infrastructure.Services.Webinars
{
    public class WebinarService : IWebinarService
    {
        private readonly IWebinarRepository _webinarRepository;
        private readonly IWebinarRepository _webinarAttendeesRepository;
        private readonly IUserProvider _userProvider;
        private readonly Logger _logger;
        private readonly IMapper _mapper;
        private readonly int currentUser;
        private readonly IDateTimeService _dateTime;

        public WebinarService(IWebinarRepository WebinarRepository, IDateTimeService dateTime, IMapper mapper, IWebinarRepository webinarRepository, IWebinarRepository webinarAttendeesRepository, IUserProvider userProvider)
        {
            _webinarRepository = webinarRepository;
            _mapper = mapper;
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _webinarAttendeesRepository = webinarAttendeesRepository;
            _userProvider = userProvider;
            currentUser = _userProvider.CurrentUser.UserId;
            _dateTime = dateTime;
        }

        public async Task<List<Webinar>> GetAll()
        {
            List<Webinar> webinars = new List<Webinar>();
            try
            {
                webinars = await _webinarRepository.GetAll();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinars;
        }
        public async Task<List<Webinar>> GetAllCeo(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = await _webinarRepository.GetAllCeo(userId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetAllForAdminApproval()
        {
            List<Webinar> webinars = new List<Webinar>();
            try
            {
                webinars = (await _webinarRepository.GetAll()).Where(x => x.ApprovalStatus == ApprovalStatus.Pending && (x.Date > DateTime.Now.Date || (x.Date == DateTime.Now.Date && x.Time_From >= DateTime.Now.TimeOfDay)))
                    .OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinars;
        }
        public async Task<List<Webinar>> GetArchived()
        {
            List<Webinar> webinars = new List<Webinar>();
            try
            {
                webinars = await _webinarRepository.GetArchived();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinars;
        }

        public async Task<List<Webinar>> GetPendingApprovals()
        {
            List<Webinar> webinars = new List<Webinar>();
            try
            {
                webinars = await _webinarRepository.GetPendingApprovals();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinars;
        }

        public async Task<Webinar> GetById(int id)
        {
            Webinar webinar = new Webinar();
            try
            {
               webinar = await _webinarRepository.GetById(id);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }

        public async Task<List<Webinar>> GetAllByEmployeeId(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = await _webinarRepository.GetAllByEmployeeId(userId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetPendingByEmployeeId(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = (await _webinarRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Pending && (x.Date > DateTime.Now.Date || (x.Date == DateTime.Now.Date && x.Time_From >= DateTime.Now.TimeOfDay))).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }

        public async Task<List<Webinar>> GetScheduledByEmployeeId(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = (await _webinarRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date > DateTime.Now.Date).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }

        public async Task<List<Webinar>> GetTodaysWebinarsByEmployeeId(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = (await _webinarRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus == ApprovalStatus.Approved && x.Date == DateTime.Now.Date).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetArchivedByEmployeeId(int userId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = (await _webinarRepository.GetAllByEmployeeId(userId)).Where(x => x.ApprovalStatus != ApprovalStatus.Pending && x.Date + x.Time_From < DateTime.Now).OrderByDescending(x => x.Date).ThenByDescending(x => x.Time_From).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }

        public async Task<List<Webinar>> GetByEmployeeIdAsHost(int currentUserId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = await _webinarRepository.GetByEmployeeIdAsHost(currentUserId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetByEmployeeIdAsAttendee(int currentUserId)
        {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                webinar = await _webinarRepository.GetByEmployeeIdAsAttendee(currentUserId);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetByStatus(ApprovalStatus status) {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                var entity = (await _webinarRepository.GetByEmployeeIdAsHost(currentUser)).Where(x=>x.ApprovalStatus == status).ToList();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }
        public async Task<List<Webinar>> GetAllByStatus(ApprovalStatus status) {
            List<Webinar> webinar = new List<Webinar>();
            try
            {
                var entity = (await _webinarRepository.GetAll()).Where(x => x.ApprovalStatus == status).ToList();            
            }
            catch (Exception Ex)
            {
                throw;
            }

            return webinar;
        }


        public async Task<int> Create(Webinar webinar, CancellationToken cancellationToken)
        {
            int result = 0;
            try
            {
                result =  await _webinarRepository.Create(webinar, cancellationToken);
            }
            catch (Exception Ex)
            {
                _logger.Error(Ex.Message);
                throw;
            }

            return result;
        }

        //public async Task<MeetingAvailability> CheckIfWebinarTimeAvailableForHost( DateTime date, TimeSpan from, TimeSpan to, int hostId, int webinarId = 0)
        //{
        //    bool result = false;
        //    try
        //    {
        //        var webinarCanBeReserved = (await _webinarRepository.GetAll()).Where(x => x.Id != webinarId && x.Date+x.Time_From >= _dateTime.Now && (x.ApprovalStatus == ApprovalStatus.Approved || x.ApprovalStatus == ApprovalStatus.Pending)).ToList();

        //        var hostNotAvailableForSpecifiedTime = webinarCanBeReserved.Any(
        //                                        x =>
        //                                        x.Id != webinarId &&
        //                                        x.Date == date
        //                                        && (x.HostId == hostId || x.WebinarPanelists.Any(e => e.EmployeeId == hostId))
        //                                        && x.IsActive == true
        //                                        && ((((x.Time_From >= from && x.Time_From <= to) || (x.Time_To >= from && x.Time_To <= to)))));

        //        if (hostNotAvailableForSpecifiedTime)
        //        {
        //            return MeetingAvailability.HostNotAvailable;
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw;
        //    }

        //    return MeetingAvailability.Available;
        //}

        public async Task<int> Update(Webinar webinar)
        {
            var result = 0;
            try
            {
                CancellationToken cancellation = new CancellationToken();
                result = await _webinarRepository.Update(webinar, cancellation);
            }
            catch (Exception Ex)
            {
                throw;
            }

            return result;
        }

//        public async Task<int> CancelWebinar(int id)
//        {
//            Webinar entity = new Webinar();
//            var webinar = await _webinarRepository.GetById(id);

//            CancellationToken cancellation = new CancellationToken();
//            try
//            {
//=
//                 webinar   
//                }
//                entity = _mapper.Map<Webinar>(webinar);
//            }
//            catch (Exception Ex)
//            {
//                throw;
//            }

//            return await _webinarRepository.Update(entity, cancellation);
//        }

        public async Task<int> Delete(int id, ApprovalStatus status, CancellationToken cancellationToken)
        {
            return await _webinarRepository.Delete(id, status, cancellationToken);
        }


        public async Task<int> Cancel(int id, CancellationToken cancellationToken)
        {
            return await _webinarRepository.Cancel(id, cancellationToken);
        }
    }
}
