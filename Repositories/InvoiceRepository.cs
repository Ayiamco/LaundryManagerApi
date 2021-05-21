using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Infrastructure;
using AutoMapper;
using LaundryApi.Entites;
using Microsoft.EntityFrameworkCore;
using LaundryApi.Models;

namespace LaundryApi.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        //private readonly LaundryApiContext _context;
        //private readonly IMapper mapper;
        //private readonly IRepositoryHelper repositoryHelper;

        //public InvoiceRepository(LaundryApiContext _context, IMapper mapper,IRepositoryHelper repositoryHelper)

        //{
        //    this._context = _context;
        //    this.mapper = mapper;
        //    this.repositoryHelper = repositoryHelper;
        //}

        //public async Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        //{

        //    var invoice = await _context.Invoices.Include(x=> x.Customer)
        //                                         .Include(x=> x.InvoiceItems)
        //                                         .ThenInclude(x=> x.Service)
        //                                         .FirstOrDefaultAsync(x => x.Id == invoiceId);
        //    var invoiceDto = mapper.Map<InvoiceDto>(invoice);
        //    return invoiceDto;
        //}

        //public InvoiceDto AddInvoice(NewInvoiceDto newInvoiceDto, string userRole,string username)
        //{
        //    try
        //    {
        //        var customerInDb = _context.Customers.SingleOrDefault(x => x.Id == newInvoiceDto.CustomerId);
        //        if (customerInDb == null)
        //            throw new Exception(ErrorMessage.EntityDoesNotExist);

        //        if (userRole == RoleNames.LaundryOwner)
        //        {
        //            if ( repositoryHelper.GetLaundryByUsername(username).Id != customerInDb.LaundryId)
        //                throw new Exception(ErrorMessage.InvalidToken);
        //        }
        //        else
        //        {
        //            if (repositoryHelper.GetEmployeeByUsername(username).LaundryId != customerInDb.LaundryId)
        //                throw new Exception(ErrorMessage.InvalidToken);
        //        }

        //        var invoiceTotal = GetInvoiceItemsPrices(newInvoiceDto,customerInDb.LaundryId).Sum();

        //        customerInDb.Debt += invoiceTotal - newInvoiceDto.AmountPaid;

        //        if (newInvoiceDto.AmountPaid > 0)
        //        {
        //            customerInDb.TotalPurchase += newInvoiceDto.AmountPaid;
        //            Laundry laundry = _context.Laundries.Find(customerInDb.LaundryId);
        //            laundry.Revenue += newInvoiceDto.AmountPaid;
        //            if(customerInDb.EmployeeId != new Guid())
        //            {
        //                Employee employee = _context.Employees.Find(customerInDb.EmployeeId);
        //                employee.Revenue += newInvoiceDto.AmountPaid;
        //            }
        //        }

        //        //create the invoice object and add the invoice to the db context
        //        var invoiceItems=mapper.Map<ICollection<InvoiceItem>>(newInvoiceDto.InvoiceItems);
        //        Invoice invoice = new Invoice()
        //        {
        //            Amount = invoiceTotal,
        //            CustomerId = newInvoiceDto.CustomerId,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now,
        //            IsCollected = false,
        //            IsPaidFor = customerInDb.Debt<=0,
        //            AmountPaid= newInvoiceDto.AmountPaid,
        //            Remark=newInvoiceDto.Remark,
        //            InvoiceItems= invoiceItems,
        //            LaundryId= customerInDb.LaundryId
        //        };
        //        _context.Invoices.Add(invoice);

        //        //coomplete transaction
        //        _context.SaveChanges();

        //        //create return obj
        //        InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
        //        invoiceDto.CustomerId = newInvoiceDto.CustomerId;

        //        return invoiceDto;
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message == ErrorMessage.EntityDoesNotExist)
        //            throw new Exception(ErrorMessage.EntityDoesNotExist);

        //        else if (e.Message == ErrorMessage.InvalidToken)
        //            throw new Exception(ErrorMessage.InvalidToken);

        //        throw new Exception(ErrorMessage.FailedDbOperation);
        //    }


        //}

        //public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize)
        //{
        //    var invoicesList = _context.Invoices.ToList();
        //    var page=invoicesList.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        //    PagedList<InvoiceDto> obj = new PagedList<InvoiceDto>()
        //    {
        //        Data= mapper.Map<IEnumerable<InvoiceDto>>(page),
        //        PageIndex=pageNumber,
        //        PageSize=pageSize,
        //        MaxPageIndex=invoicesList.Count,
        //    };

        //    return obj;
        //}
        //public PagedList<Invoice> GetInvoices(int pageNumber, int pageSize,string username,string userRole)
        //{
        //    //get the laundry 
        //    Guid laundryId;
        //    if (userRole == RoleNames.LaundryEmployee)
        //        laundryId = _context.Employees.SingleOrDefault(x => x.Username == username).LaundryId;
        //    else
        //        laundryId = _context.Laundries.SingleOrDefault(x => x.Username == username).Id;

        //    //get the laundry customers and use the customers to get the invoices from the invoice table
        //    IEnumerable<Invoice> invoices = _context.Invoices.Include(x=>x.Customer).Include(x=> x.InvoiceItems).Where(x => x.LaundryId == laundryId);


        //    //do the pagination and select 
        //    var page = invoices.OrderBy(x => x.Amount).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        //    PagedList<Invoice> obj = new PagedList<Invoice>()
        //    {
        //        Data = page,
        //        PageIndex = pageNumber,
        //        PageSize = pageSize,
        //        MaxPageIndex = invoices.Count(),
        //    };

        //    return obj;
        //}

        //public InvoiceDto ReadCompleteInvoiceAsync(Guid invoiceId)
        //{
        //    try
        //    {
        //        var invoice =  _context.Invoices.Include(x => x.Customer).Include(x => x.InvoiceItems).SingleOrDefault(x => x.Id==invoiceId);
        //        InvoiceDto invoiceDto = mapper.Map<InvoiceDto>(invoice);
        //        return invoiceDto;
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message == ErrorMessage.EntityDoesNotExist)
        //            throw new Exception(ErrorMessage.EntityDoesNotExist);

        //        throw new Exception(ErrorMessage.FailedDbOperation);
        //    }



        //}

        //public void DepositCustomerPayment(Guid customerId, decimal amtDeposited)
        //{
        //    IEnumerable<Invoice> invoices = _context.Invoices
        //        .Where(x => x.CustomerId == customerId && !x.IsPaidFor).OrderBy(x => x.CreatedAt);
        //    if (invoices.Count() == 0)
        //        throw new Exception(ErrorMessage.CustomerIsNotOwing);

        //    //loop thru the invoices to find unpaid invoices
        //    UpdateInvoicesWIthDeposit(invoices, amtDeposited);

        //    //update the laundry,employee and customer object
        //    var customer = _context.Customers.Find(customerId);
        //    customer.TotalPurchase += amtDeposited;
        //    if(customer.EmployeeId != new Guid())
        //    {
        //        var employee = _context.Employees.Find(customer.EmployeeId);
        //        employee.Revenue += amtDeposited;
        //    }
        //    Laundry laundry=_context.Laundries.Find(customer.LaundryId);
        //    laundry.Revenue += amtDeposited;

        //    _context.SaveChanges();

        //}

        //public IEnumerable<InvoiceDto> FetchCustomerInvoices(Guid customerId)
        //{
        //   var invoices= _context.Invoices.Where(x => x.CustomerId == customerId).OrderBy(x => x.CreatedAt);
        //    var invoiceDtos = mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        //    return invoiceDtos;

        //}

        //private static void UpdateInvoicesWIthDeposit(IEnumerable<Invoice> invoices,decimal amtDeposited)
        //{
        //    foreach (Invoice invoice in invoices)
        //    {
        //        decimal invoiceBalance = invoice.Amount - invoice.AmountPaid;
        //        if (invoiceBalance >= amtDeposited)
        //        {
        //            invoice.AmountPaid += amtDeposited;
        //            amtDeposited = 0;
        //        }
        //        else if (invoiceBalance < amtDeposited)
        //        {
        //            invoice.AmountPaid += invoiceBalance;
        //            amtDeposited -= invoiceBalance;
        //            invoice.IsPaidFor = true;
        //        }

        //        if (amtDeposited == 0)
        //            break;

        //    }
        //}

        //private  IEnumerable<decimal> GetInvoiceItemsPrices(NewInvoiceDto dto,Guid laundryId )
        //{
        //    var services = _context.Services.Where(x => x.LaundryId == laundryId);
        //    foreach(NewInvoiceItemDto _dto in dto.InvoiceItems)
        //        yield return services.Single(x=> x.Id==_dto.ServiceId).Price * _dto.Quantity;
        //}
        public InvoiceDto AddInvoice(NewInvoiceDto newInvoice, string userRole, string applicationUserUsername)
        {
            throw new NotImplementedException();
        }

        public void DepositCustomerPayment(Guid customerId, decimal amtDeposited)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InvoiceDto> FetchCustomerInvoices(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public PagedList<InvoiceDto> GetInvoices(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PagedList<Invoice> GetInvoices(int pageNumber, int pageSize, string username, string userRole)
        {
            throw new NotImplementedException();
        }

        public InvoiceDto ReadCompleteInvoiceAsync(Guid invoiceId)
        {
            throw new NotImplementedException();
        }

        public Task<InvoiceDto> ReadInvoice(Guid invoiceId)
        {
            throw new NotImplementedException();
        }
    }
}