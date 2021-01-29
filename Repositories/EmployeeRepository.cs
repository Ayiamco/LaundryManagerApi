﻿using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
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
                
                var employee = await _context.Employees.FindAsync(id);
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

        public async Task<EmployeeDto> CreateEmployeeAsync(NewEmployeeDto newEmployeeDto)
        {
            try
            {
                //update new employee info
                var employee = mapper.Map<Employee>(newEmployeeDto);
                employee.PasswordHash = HashPassword(newEmployeeDto.Password);
                employee.CreatedAt = DateTime.Now;
                employee.UpdatedAt = DateTime.Now;
                employee.ForgotPasswordTime = null;
                employee.PasswordResetId = null;


                //add employee to db context
                await _context.Employees.AddAsync(employee);

                ////assign role to employee
                //Role role = _context.Roles.SingleOrDefault(x => x.Name == RoleNames.LaundryEmployee);
                //var userRole = new UserRole() { ApplicationUserId = employee.Id, RoleId = role.Id };
                //await _context.UsersRoles.AddAsync(userRole);

                //complete db transaction 
                await _context.SaveChangesAsync();

                //update employeeDto object
                var employeeDto = mapper.Map<EmployeeDto>(employee);
                employeeDto.Password = newEmployeeDto.Password;
                return employeeDto;
            }
            catch (Exception e)
            {
                if (e.InnerException.ToString().Contains("Cannot insert duplicate key row in object 'dbo.Employees'"))
                    throw new Exception(ErrorMessage.UsernameAlreadyExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

        public async Task<bool> DeleteEmployee (Guid employeeId,string laundryUsername)
        {
            try
            {
                Employee employeeInDb= await ValidateRequestParam(employeeId, laundryUsername);

                employeeInDb.IsDeleted = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EmployeeNotOwnedByUser)
                    throw new Exception(ErrorMessage.EmployeeNotOwnedByUser);

                else if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public async Task<EmployeeDto> UpdateEmployee(EmployeeDto employee,string laundryUsername)
        {
            Employee employeeInDb=await ValidateRequestParam(employee.Id, laundryUsername);

            employeeInDb.Address = employee.Address;
            employeeInDb.Name = employee.Name;
            employeeInDb.PhoneNumber = employee.PhoneNumber;
            employeeInDb.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            employee = mapper.Map<EmployeeDto>(employeeInDb);
            employee.Laundry = null;
            return employee;
        }

        private async Task<Employee> ValidateRequestParam(Guid employeeId, string laundryUsername)
        {
            Laundry laundry = _context.Laundries.SingleOrDefault(x => x.Username == laundryUsername);
            Employee employeeInDb = await _context.Employees.FindAsync(employeeId);
            if (employeeInDb == null)
                throw new Exception(ErrorMessage.UserDoesNotExist);

            if (employeeInDb.LaundryId != laundry.Id)
                throw new Exception(ErrorMessage.EmployeeNotOwnedByUser);

            return employeeInDb;
        }

    }
}
