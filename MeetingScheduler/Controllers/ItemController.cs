using AutoMapper;
using MeetingScheduler.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.UI.Controllers;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Services.MeetingItems;
using MeetingScheduler.UI.Models;

namespace MeetingScheduler.UI.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;
        private readonly IUserProvider _currentUser;

        public ItemController(IItemService itemService, IMapper mapper, IUserProvider currentUser)
        {
            _itemService = itemService;
            _mapper = mapper;
            _currentUser = currentUser;

        }

        public async Task<ActionResult> Index()
        {
            List<ItemDto> result;
            List<ItemVm> itemsVm = new List<ItemVm>();
            try
            {
                result = await _itemService.GetAll();
                itemsVm = _mapper.Map<List<ItemVm>>(result);
            }
            catch
            {
                throw;
            }
            return View(itemsVm);

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
        public async Task<ActionResult> Create(ItemVm user, CancellationToken cancellationToken)
        {
            var result = 0;
            try
            {

                if (!ModelState.IsValid)
                {
                    return View("Create", user);
                }

                ItemDto userDto = _mapper.Map<ItemDto>(user);
                result = await _itemService.Create(userDto, cancellationToken);
            }
            catch
            {
                throw;
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            ItemVm userVm;
            try
            {
                var result = await _itemService.GetById(id);
                userVm = _mapper.Map<ItemVm>(result);
            }
            catch
            {
                throw;
            }
            return View(userVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ItemVm user)
        {
            var result = 0;
            try
            {
                ItemDto userDto = _mapper.Map<ItemDto>(user);
                CancellationToken token = new CancellationToken();
                result = await _itemService.Update(userDto, token);
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
                result = await _itemService.Delete(id, cancellationToken);
            }
            catch
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
