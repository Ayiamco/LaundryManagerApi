using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Models;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Infrastructure;

namespace LaundryApi.Repositories
{
    public class InvoiceItemRepository:IInvoiceItemRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public InvoiceItemRepository(IMapper mapper,LaundryApiContext context)
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
