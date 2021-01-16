using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Models;
using static LaundryApi.Services.HelperMethods;

namespace LaundryApi.Services
{
    public class InvoiceItemDbService:IInvoiceItemDbService
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public InvoiceItemDbService(IMapper mapper,LaundryApiContext context)
        {
            this.mapper = mapper;
            _context = context;
        }
        public async void AddInvoiceItem(List<InvoiceItemDto> invoiceItemDtoList)
        {
            try
            {
                List<InvoiceItem> invoiceItemList= new List<InvoiceItem>();
                invoiceItemDtoList.ForEach(item =>
                {
                    invoiceItemList.Add(mapper.Map<InvoiceItem>(item));
                });
                await _context.InvoiceItems.AddRangeAsync(invoiceItemList);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
            
        }
    }
}
