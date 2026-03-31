using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployees();
        Task<List<Employee>> GetSystemAdmins();
        Task<Employee> GetEmployeeById(int Id);
        Task<Employee> GetEmployeeByGuid(Guid guid);
        Task<int> Create(Employee employee, CancellationToken cancellationToken);
        Task<int> Update(Employee employee, CancellationToken cancellationToken);
        Task<List<Role>> GetRoles();
        Task<int> SaveADEmployeesToDatabase(List<Employee> employee);

    }
}
