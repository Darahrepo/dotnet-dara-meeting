using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Services.MeetingRooms;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Controllers
{
    [Authorize(Policy = "FullAccess")]
    public class MeetingRoomController : BaseController
    {
        private readonly IMeetingRoomService _meetingRoomService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResource;

        public MeetingRoomController(IMeetingRoomService meetingRoomService, IStringLocalizer<SharedResource> sharedResource, IMapper mapper)
        {
            _meetingRoomService = meetingRoomService;
            _mapper = mapper;
            _sharedResource = sharedResource;
        }

        public async Task<ActionResult> Index(ToastVm toastr = null)
        {
            List<MeetingRoom> result;
            List<MeetingRoomVm> meetingRoomsVm = new List<MeetingRoomVm>();
            if (!string.IsNullOrEmpty(toastr.Message))
            {
                TempData["type"] = toastr.Type.ToString();
                TempData["message"] = toastr.Message;
            }
            try
            {
                result = await _meetingRoomService.GetAll();
                meetingRoomsVm = _mapper.Map<List<MeetingRoomVm>>(result);
            }
            catch
            {
                throw;
            }
            return View(meetingRoomsVm);

        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MeetingRoomVm user, CancellationToken cancellationToken)
        {
            var result = 0;
            ToastVm toastr = new ToastVm();
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Create", user);
                }

                MeetingRoom userDto = _mapper.Map<MeetingRoom>(user);
                result = await _meetingRoomService.Create(userDto, cancellationToken);
            }
            catch
            {
                throw;
            }
            if (result > 0)
            {
                toastr.Type = ToastAlertType.success;
                toastr.Message = _sharedResource["CreateSuccess"];
            }
            else
            {
                toastr.Type = ToastAlertType.error;
                toastr.Message = _sharedResource["CreateError"];
            }

            return RedirectToAction("Index", toastr);

        }

        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            MeetingRoomVm userVm;
            try
            {
                var result = await _meetingRoomService.GetById(id);
                userVm = _mapper.Map<MeetingRoomVm>(result);
            }
            catch
            {
                throw;
            }
            return View(userVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, MeetingRoomVm user)
        {
            var result = 0;
            ToastVm toastr = new ToastVm();

            try
            {
                MeetingRoom userDto = _mapper.Map<MeetingRoom>(user);
                CancellationToken token = new CancellationToken();
                result = await _meetingRoomService.Update(userDto, token);
            }
            catch
            {
                throw;
            }

            if (result > 0)
            {
                toastr.Type = ToastAlertType.success;
                toastr.Message = _sharedResource["EditSuccess"];
            }
            else
            {
                toastr.Type = ToastAlertType.error;
                toastr.Message = _sharedResource["EditError"];
            }

            return RedirectToAction("Index", toastr);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var result = 0;
            try
            {
                CancellationToken cancellationToken = new CancellationToken();
                result = await _meetingRoomService.Delete(id, cancellationToken);
            }
            catch
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
