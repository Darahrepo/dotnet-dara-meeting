using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Services.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeScheduler.Infrastructure.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployees();  
        Task<List<Employee>> GetSystemAdmins(); 
         Task<Employee> GetEmployeeById(int id);
        Task<Employee> GetEmployeeByGuid(Guid guid);
        Task<int> CreateEmployee(Employee Employee, CancellationToken cancellationToken);
        Task<int> Update(Employee sampleUser, CancellationToken cancellationToken);
        Task<List<Role>> GetRoles();
        Task<int> SaveADEmployeesToDatabase(List<AdUser> Employees);
    }
}
