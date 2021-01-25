using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Interfaces;

namespace LaundryApi.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public EmployeeRepository(LaundryApiContext _context, IMapper mapper)
        {
            this._context = _context;
            this.mapper = mapper;
        }

       

        public async Task<EmployeeDto> FindEmployeeAsync(Guid id)
        {
            try
            {
                
                var employee = await _context.ApplicationUsers.FindAsync(id);
                if (employee == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                var employeeDto = mapper.Map<EmployeeDto>(employee);
                return employeeDto;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }


    }
}
