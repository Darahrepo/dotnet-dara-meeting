using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Enums;

namespace MeetingScheduler.Domain.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserClaimsService  _userClaimService;

        public EmployeeRepository(IApplicationDbContext context, IUserClaimsService userClaimService)
        {
            _context = context;
            _userClaimService = userClaimService;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees.Where(x => x.IsActive == true).Include(x=>x.Role).ToListAsync();
        }
        public async Task<List<Employee>> GetSystemAdmins()
        {
            return await _context.Employees.Where(x => x.IsActive == true && x.RoleId == Convert.ToInt32(Roles.Admin)).Include(x => x.Role).ToListAsync();
        }
        public async Task<Employee> GetEmployeeById(int id)
        {
            Employee result = new Employee();
            try {
                result = await _context.Employees.Where(x => x.Id == id && x.IsActive == true).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public async Task<Employee> GetEmployeeByGuid(Guid guid)
        {
            Employee result = null;
            try
            {

                result = await _context.Employees.Where(x => x.Guid == guid && x.IsActive == true).Include(x=>x.Role).SingleOrDefaultAsync();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return result;
        }

        public async Task<int> Create(Employee employee, CancellationToken cancellationToken)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception Ex)
            {
                throw;
            }
            return employee.Id;
        }

        public async Task<int> Update(Employee employee, CancellationToken cancellationToken)
        {
            var result = 0;
            try
            {
                var entity = await _context.Employees.FindAsync(employee.Id);
                bool changeRole = false;
                if (entity == null)
                {
                    throw new NotFoundException(nameof(Employee), employee.Id);
                }

                entity.FirstNameAr = employee.FirstNameAr;
                entity.FirstNameEn = employee.FirstNameEn;

                entity.MiddleNameAr = employee.MiddleNameAr;
                entity.MiddleNameEn = employee.MiddleNameEn;

                entity.LastNameAr = employee.LastNameAr;
                entity.LastNameEn = employee.LastNameEn;
                if (employee.RoleId != entity.RoleId)
                {
                    changeRole = true;
                }
                entity.RoleId = employee.RoleId;

                result = await _context.SaveChangesAsync(cancellationToken);

                //if (result > 0 && changeRole == true)
                //{
                //    await _userClaimService.AddUpdateClaim("Role", employee.RoleId.ToString());
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }

        public async Task<List<Role>> GetRoles()
        {
            var entity = await _context.Roles.ToListAsync();
            return entity;
        }

        public async Task<int> SaveADEmployeesToDatabase(List<Employee> employees)
        {
            CancellationToken cancellationToken = new CancellationToken();
            foreach(var emp in employees)
            {
                bool res = _context.Employees.Any(x => x.Guid == emp.Guid);
                if(!res)
                {
                    _context.Employees.Add(emp);
                }
            }
            
            var entity = await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

    }
}
