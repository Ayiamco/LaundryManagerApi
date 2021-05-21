using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Entites;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Repositories
{
    public class DashboardRepo : IDashboardRepo
    {
        //private readonly LaundryApiContext _context;
        //private IMapper _mapper;
        //public DashboardRepo(LaundryApiContext context,IMapper mapper)
        //{
        //    _context = context;
        //    _mapper = mapper;
        //}

        //public  DashboardDto GetDashboardData(string username)
        //{
        //    var laundry=_context.Laundries.SingleOrDefault(x => x.Username == username);
        //    var employees = _context.Employees.Where(x => x.LaundryId == laundry.Id);
        //    var invoices = _context.Invoices.Where(x => x.LaundryId == laundry.Id).AsQueryable();
        //    var customers = _context.Customers.Where(x => x.LaundryId == laundry.Id).OrderBy(x=> x.TotalPurchase).AsQueryable();
        //    var services = _context.Services.Where(x => x.LaundryId == laundry.Id).OrderBy(x=> x.Revenue).AsQueryable();
        //    var topService = services.Count() != 0 ? 
        //        _mapper.Map<ServiceDto>(services.First()).Name : "Not Available" ;
        //    var dto = new DashboardDto
        //    {
        //        Name = laundry.Name,
        //        InvoiceAmount = invoices.Select(x => x.Amount).Count()==0 ? 0 : invoices.Select(x => x.Amount).Sum(),
        //        InvoiceCount = invoices.Count(),
        //        CustomerCount = customers.Count(),
        //        TopService= topService,
        //        NoOfEmployees= employees.Count(),
        //        Revenue=invoices.Select(x=> x.AmountPaid).Count()==0?0: invoices.Select(x => x.AmountPaid).Sum()
        //    };
        //    return dto;
        //}
        public DashboardDto GetDashboardData(string username)
        {
            throw new NotImplementedException();
        }
    }
}
