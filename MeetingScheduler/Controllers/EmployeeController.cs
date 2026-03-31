using AutoMapper;
using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Infrastructure.Services.Employees;
using MeetingScheduler.UI.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResource;
        private readonly IStringLocalizer<EmployeeController> _employeeControllerResource;
        public EmployeeController(IUserProvider userProvider, IStringLocalizer<SharedResource> sharedResource, IEmployeeService employeeService, IMapper mapper, IStringLocalizer<EmployeeController> employeeControllerResource)
        {
            _userProvider = userProvider;
            _employeeService = employeeService;
            _mapper = mapper;
            _sharedResource = sharedResource;
            _employeeControllerResource = employeeControllerResource;
        }


        public async Task<ActionResult> Index(ToastVm toastr = null)
        {
            List<Employee> result = new List<Employee>();
            List<EmployeeVm> employeesVm = new List<EmployeeVm>();


            if (!string.IsNullOrEmpty(toastr.Message))
            {
                TempData["type"] = "" + toastr.Type.ToString();
                TempData["message"] = toastr.Message;
            }


            try
            {
                result = await _employeeService.GetAllEmployees();
                employeesVm = _mapper.Map<List<EmployeeVm>>(result);
            }
            catch
            {
                throw;
            }
            return View(employeesVm);
        }


        public async Task<ActionResult> Get()
        {
            List<Employee> result = new List<Employee>();
            List<EmployeeVm> employeesVm = new List<EmployeeVm>();
            try
            {
                result = await _employeeService.GetAllEmployees();
                employeesVm = _mapper.Map<List<EmployeeVm>>(result);
            }
            catch
            {
                throw;
            }
            return View(employeesVm);
        }


        public ActionResult Details(int id)
        {
            return View();
        }


        //public ActionResult Create()
        //{
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(EmployeeVm user,CancellationToken cancellation)
        //{
        //    var result = 0;
        //    try
        //    {

        //        if (!ModelState.IsValid)
        //        {
        //            return View("Create", user);
        //        }

        //        Employee userObj = _mapper.Map<Employee>(user);
        //        result = await _employeeService.CreateEmployee(userObj, cancellation);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return RedirectToAction(nameof(Index));

        //}


        public async Task<ActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            EmployeeVm userVm;
            try
            {
                var result = await _employeeService.GetEmployeeById(id);

                userVm = _mapper.Map<EmployeeVm>(result);

                userVm.Roles = await _employeeService.GetRoles();
            }
            catch
            {
                throw;
            }
            return View(userVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EmployeeVm user)
        {
            var result = 0;
            ToastVm toastr = new ToastVm();
            try
            {
                Employee userObj = _mapper.Map<Employee>(user);
                CancellationToken token = new CancellationToken();
                result = await _employeeService.Update(userObj, token);
            }
            catch
            {
                throw;
            }

            if (result > 0 )
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


        public async Task<ActionResult> AddADEmployeesToDatabase()
        {
            var employees = await _userProvider.GetDomainUsers();
            var result = await _employeeService.SaveADEmployeesToDatabase(employees);
            ToastVm toastr = new ToastVm();
            if (result > 0)
            {
                toastr.Type = ToastAlertType.success;
                toastr.Message = String.Format(_sharedResource["SyncSuccess"], result);
            }
            else
            {
                toastr.Type = ToastAlertType.warning;
                toastr.Message = _sharedResource["EmployeesUpToDate"];
            }

            return RedirectToAction("Index", toastr);
        }
    }
}
