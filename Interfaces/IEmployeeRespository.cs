using LaundryApi.Dtos;
using LaundryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IEmployeeRepository
    {
        public Task<EmployeeDto> FindEmployeeAsync(Guid employeeId);
        public Task<EmployeeDto> CreateEmployeeAsync(NewEmployeeDto newEmployeeDto);

        public Task<bool> DeleteEmployee(Guid employeeId, string laundryUsername);

        public Task<EmployeeDto> UpdateEmployee(EmployeeDto employee, string laundryUsername);

        public  Task<bool> SendEmployeeRegistrationLink(string employeeEmail, string laundryName);

        public bool IsEmployeeInTransit(string employeeEmail, Guid laundryId);
        public PagedList<EmployeeDtoPartial> GetPage(int pageSize, string laundryUsername, int pageNumber = 1, string searchParam = "");


    }
}
