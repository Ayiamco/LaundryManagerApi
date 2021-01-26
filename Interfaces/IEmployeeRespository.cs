using LaundryApi.Dtos;
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
    }
}
