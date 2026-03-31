using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Infrastructure.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Employees
{
    public class EmployeeDto : IMapFrom<Employee>
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Guid Guid { get; set; }
        public string DisplayName { get; set; }
        public string FirstNameEn { get; set; }
        public string MiddleNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string MiddleNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string EmailAddress { get; set; }
        public string DepartmentId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EmployeeDto, Employee>(MemberList.Destination).ReverseMap();

        }
    }
}
