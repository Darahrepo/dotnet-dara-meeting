using AutoMapper;
using EmployeeScheduler.Infrastructure.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Identity;
using MeetingScheduler.Infrastructure.Services.Employees;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeScheduler.Infrastructure.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository EmployeeRepository, IMapper mapper, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            var entity = await _employeeRepository.GetAllEmployees();
            List<Employee> employees = _mapper.Map<List<Employee>>(entity);

            return employees;
        }
        public async Task<List<Employee>> GetSystemAdmins()
        {
            var entity = await _employeeRepository.GetSystemAdmins();
            List<Employee> employees = _mapper.Map<List<Employee>>(entity);

            return employees;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            Employee employee = new Employee();
            try
            {

                var entity = await _employeeRepository.GetEmployeeById(id);
                employee = _mapper.Map<Employee>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }

            return employee;
        }
        
        public async Task<Employee> GetEmployeeByGuid(Guid guid)
        {
            var entity = await _employeeRepository.GetEmployeeByGuid(guid);
            Employee employee = _mapper.Map<Employee>(entity);

            return employee;
        }

        public async Task<int> CreateEmployee(Employee employee, CancellationToken cancellationToken)
        {
            int result = 0;
            try
            {

                Employee entity = _mapper.Map<Employee>(employee);
                result =  await _employeeRepository.Create(entity, cancellationToken);
            }
            catch (Exception Ex)
            {
                throw;
            }
            return result;
        }

        public async Task<int> Update(Employee employee, CancellationToken cancellationToken)
        {
            Employee entity = _mapper.Map<Employee>(employee);

            return await _employeeRepository.Update(entity, cancellationToken);
        }

        public async Task<List<Role>> GetRoles()
        {
            List<Role> role = await _employeeRepository.GetRoles();
            return role;
        }

        public async Task<int> SaveADEmployeesToDatabase(List<AdUser> employees)
        {
            // "OU=YourDepartment,DC=au,DC=company,DC=com"
            List<Employee> allEmployees = new List<Employee>();
            foreach(var emp in employees)
            {
                Employee employee = new Employee();
                employee.DisplayName = emp.DisplayName??"";
                employee.EmailAddress = emp.Email;
                employee.FirstNameEn = employee.DisplayName.Split(" ")[0];
                employee.MiddleNameEn = employee.DisplayName.Split(" ").Length > 1 ? employee.DisplayName.Split(" ")[1] : null;
                employee.LastNameEn = employee.DisplayName.Split(" ").Length > 2 ? employee.DisplayName.Split(" ")[emp.DisplayName.Split(" ").Length - 1] : null;
                employee.Guid = emp.Guid.Value;
                allEmployees.Add(employee);
            }

            return await _employeeRepository.SaveADEmployeesToDatabase(allEmployees);
        }
    }
}
