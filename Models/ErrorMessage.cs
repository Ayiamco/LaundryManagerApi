using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public  struct ErrorMessage
    {
        public const string InvalidToken = "invalid jwt token";
        public const string FailedDbOperation = "failed to access database";
        public const string UserDoesNotExist = "user does not exist ";
        public const string EntityDoesNotExist = "data was not found ";
        public const string UsernameAlreadyExist = "user email already exist";
        public const string ServiceAlreadyExist = "service already exist";
        public const string LinkExpired = "link has expired";
        public const string InCorrectPassword = "password is incorrect";
        public const string InvalidModel = "reqeust body is invalid";
        public const string UserHasTwoRoles = "username is tied to two roles";
        public const string EmployeeNotOwnedByUser = "employee is not owned by the current user";
        public const string NoEntityMatchesSearch = " could not find any item that matches the search";
        public const string OnlyLaundryOwnerAllowed = "only laundry owners can access this method";
    }
}
