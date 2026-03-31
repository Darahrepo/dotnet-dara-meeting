using AutoMapper;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Services.SampleUsers;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Controllers
{
    [Authorize]
    public class SampleUserController : BaseController
    {
        private readonly ISampleUserService _sampleUserService;
        private readonly IMapper _mapper;
        private readonly IUserProvider _currentUser;

        public SampleUserController(ISampleUserService sampleUserService, IMapper mapper, IUserProvider currentUser)
        {
            _sampleUserService = sampleUserService;
            _mapper = mapper;
            _currentUser = currentUser;

        }

        public async Task<ActionResult<List<SampleUserVm>>> Index()
        {
            List<SampleUserDto> result;
            List<SampleUserVm> sampleUsersVm = new List<SampleUserVm>();
            try
            {
                result = await _sampleUserService.GetAll();
                sampleUsersVm = _mapper.Map<List<SampleUserVm>>(result);
            }
            catch
            {
                throw;
            }
            return View(sampleUsersVm);

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
        public async Task<ActionResult> Create(SampleUserVm user, CancellationToken cancellationToken) {
            var result = 0;
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Create", user);
                }

                SampleUserDto userDto = _mapper.Map<SampleUserDto>(user);
                result = await _sampleUserService.Create(userDto, cancellationToken);
            }
            catch
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
           
        }

        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            SampleUserVm userVm;
            try
            {
                var result = await _sampleUserService.GetById(id);
                userVm =  _mapper.Map<SampleUserVm>(result);
            }
            catch
            {
                throw;
            }
            return View(userVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, SampleUserVm user)
        {
            var result = 0;
            try
            {
                SampleUserDto userDto = _mapper.Map<SampleUserDto>(user);
                CancellationToken token = new CancellationToken();
                result = await _sampleUserService.Update(userDto, token);
            }
            catch
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection, CancellationToken cancellationToken)
        {
            var result = 0;

            try
            {
                result = await _sampleUserService.Delete(id, cancellationToken);
            }
            catch
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
