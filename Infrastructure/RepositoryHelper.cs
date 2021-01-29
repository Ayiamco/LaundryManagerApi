using AutoMapper;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;


namespace LaundryApi.Infrastructure
{
    public class RepositoryHelper: IRepositoryHelper
    {
        private readonly LaundryApiContext _context;

        public RepositoryHelper(LaundryApiContext _context)
        {
           
            this._context = _context;
        }

        public ApplicationUser GetApplicationUser(string username)
        {
            //get user details by checking the employee and laundry tables
            var employee = _context.Employees.SingleOrDefault(x => x.Username == username);
            var laundry = _context.Laundries.SingleOrDefault(x => x.Username == username);

            //check if user exist
            if (laundry == null && employee == null)
                throw new Exception(ErrorMessage.UserDoesNotExist);

            //check if only one user role is tied to the username
            else if (laundry != null && employee != null)
                throw new Exception(ErrorMessage.UserHasTwoRoles);

            ApplicationUser user = (ApplicationUser)laundry ?? employee;
            if (user.IsDeleted)
                throw new Exception(ErrorMessage.UserDoesNotExist);
            return user;


        }

        public Laundry GetLaundryByUsername(string username)
        {
            try
            {
                var laundryInDb = _context.Laundries.SingleOrDefault(_user => _user.Username == username);

                if (laundryInDb == null || laundryInDb.IsDeleted)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                return laundryInDb;

            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);
                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public Employee GetEmployeeByUsername(string username)
        {
            try
            {
                var employeeInDb = _context.Employees.SingleOrDefault(_user => _user.Username == username);
                if (employeeInDb == null || employeeInDb.IsDeleted)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                return employeeInDb;

            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

    }
}
