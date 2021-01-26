using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;
using LaundryApi.Dtos;
using Microsoft.AspNetCore.Http;

namespace LaundryApi.Infrastructure
{
    public static class LaundryApiExtenstionMethods
    {
        //public static decimal GetInvoiceTotal(this IEnumerable<NewInvoiceItemDto> invoiceItems)
        //{
        //    decimal total = 0;
        //    foreach(NewInvoiceItemDto item in invoiceItems)
        //    {
        //        total += item.Price * item.Quantity;
        //    }
        //    return total;
        //}


        public static string GetUserRole(this HttpContext httpContext)
        {
            var currentUser = httpContext.User;
           string userRole = Convert.ToString(currentUser.Claims.SingleOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value);
            return userRole;
        }
        
        public static bool IsInRole(this HttpContext httpContext, string role)
        {
            return role == httpContext.GetUserRole();
        }
    }
}
