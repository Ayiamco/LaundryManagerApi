using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
namespace LaundryApi.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryHelper repositoryHelper;
        private readonly IMailService mailService;
        public EmployeeRepository(LaundryApiContext _context, IMapper mapper,IRepositoryHelper repositoryHelper, IMailService mailService)
        {
            this._context = _context;
            this.mapper = mapper;
            this.mailService = mailService;
            this.repositoryHelper = repositoryHelper;
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
                employee.UsernameHash = HashPassword(newEmployeeDto.Username);
                employee.ForgotPasswordTime = null;
                employee.PasswordResetId = null;

                //add employee to db context
                await _context.Employees.AddAsync(employee);

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

        public IEnumerable<EmployeeDtoPartial> GetMyEmployees(string laundryUsername)
        {
            try
            {
                Guid laundryId = repositoryHelper.GetLaundryByUsername(laundryUsername).Id;
                var employees = _context.Employees.Where(x => x.LaundryId == laundryId ).ToList();
                if (employees.Count == 0)
                    throw new Exception(ErrorMessage.NoEntityMatchesSearch);
                IEnumerable<EmployeeDtoPartial> employeeDtos = mapper.Map<IEnumerable<EmployeeDtoPartial>>(employees);

                return employeeDtos;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.NoEntityMatchesSearch)
                    throw new Exception(ErrorMessage.NoEntityMatchesSearch);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }

        public async Task<bool> SendEmployeeRegistrationLink(string employeeEmail,string laundryUsername)
        {
            Laundry laundry = _context.Laundries.SingleOrDefault(x => x.Username == laundryUsername);
            try
            {
                AddEmployeeToTransit(employeeEmail, laundry.Id);
                string url = "http://localhost:3000/employee/registration?id=" + laundry.Id;
                string mailContent = $"<p> Hi ,</p> <p> Please click <a href='{url}'>here</a> to register as an employee of {laundry.LaundryName} laundry";

                await mailService.SendMailAsync(employeeEmail, mailContent, "Employee Registration");
                return true;
            }
            catch(Exception e)
            {
                if(e.Message==ErrorMessage.UsernameAlreadyExist)
                {
                    string url = "http://localhost:3000/employee/registration?id=" + laundry.Id;
                    string mailContent = $"<p> Hi ,</p> <p> Please click <a href='{url}'>here</a> to register as an employee of {laundry.LaundryName} laundry";

                    await mailService.SendMailAsync(employeeEmail, mailContent, "Employee Registration");
                    return true;
                }
                return false;
            }
            
            
        }

        private void AddEmployeeToTransit (string employeeEmail, Guid laundryId)
        {
            try
            {
                EmployeeInTransit e = new EmployeeInTransit() { Email = employeeEmail, LaundryId = laundryId };
                _context.EmployeesInTransit.Add(e);
                _context.SaveChanges();
                return;
            }
            catch(Exception e)
            {
                if (e.InnerException.Message.Contains("Violation of UNIQUE KEY constraint 'AK_EmployeesInTransit_Email_LaundryId'"))
                    throw new Exception(ErrorMessage.UsernameAlreadyExist);
                
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
           

        }
        public  bool IsEmployeeInTransit(string employeeEmail,Guid laundryId)
        {
            return _context.EmployeesInTransit.SingleOrDefault(x=> x.Email==employeeEmail && x.LaundryId==laundryId) != null ;
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
