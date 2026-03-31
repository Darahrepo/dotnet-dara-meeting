using AutoMapper;
using EmployeeScheduler.Infrastructure.Interfaces;
using FluentValidation;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Infrastructure.Common.Extensions;
using MeetingScheduler.Infrastructure.Common.Mappings;
using MeetingScheduler.Infrastructure.Services.Employees;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MeetingScheduler.UI.Models
{
    public class EmployeeVm : IMapFrom<Employee>
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstNameEn { get; set; }
        public string MiddleNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string MiddleNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string EmailAddress { get; set; }
        public string DepartmentId { get; set; }
        public string RoleId { get; set; }
        public Role Role { get; set; }
        public string FullNameAr
        {
            get
            {
                return $"{this.FirstNameAr} {this.MiddleNameAr} {this.LastNameAr}";
            }
            set
            {
                this.FullNameAr = value;
            }
        }
        public string FullNameEn
        {
            get
            {
                return this.FirstNameEn + " " + this.MiddleNameEn + " " + this.LastNameEn;
            }
            set
            {
                this.FullNameEn = value;
            }
        }

        public List<Role> Roles {get;set;}
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Employee, EmployeeVm>().ReverseMap();


            profile.CreateMap<Employee, EmployeeVm>().ForMember(x => x.FullNameEn, opt => opt.Ignore());
            profile.CreateMap<Employee, EmployeeVm>().ForMember(x => x.FullNameAr, opt => opt.Ignore());
            profile.CreateMap<Employee, EmployeeVm>().ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }


    public class EmployeeVmValidator : AbstractValidator<EmployeeVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmployeeService _employeeService;

        public EmployeeVmValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.DisplayName)
                .NotEmpty().WithMessage("Name in English is required.")
                .MaximumLength(100).WithMessage("Name in English must not exceed 100 characters.");

            RuleFor(v => v.FirstNameEn)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(100).WithMessage("First Name must not exceed 100 characters.");

            RuleFor(v => v.Roles)
                .NotEmpty().WithMessage("Role is required.");
        }

        public async Task<bool> BeUnique(string email, CancellationToken cancellationToken)
        {
            return (await _employeeService.GetAllEmployees())
                .All(l => l.EmailAddress != email);
        }

        public async Task<bool> BeInArabic(string title, CancellationToken cancellationToken)
        {
            return await title.BeInArabic();
        }
    }
}
