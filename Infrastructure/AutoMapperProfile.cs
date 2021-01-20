using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LaundryApi.Models;
using LaundryApi.Dtos;


namespace LaundryApi.Infrastructure
{
    public class AutoMapperProfile:Profile
    {
            public AutoMapperProfile()
            {
                CreateMap<CustomerDto, Customer>().ReverseMap();
                CreateMap<NewLaundryDto, Laundry>();
                CreateMap<Laundry, Laundry>();
                CreateMap<InvoiceDto, Invoice>().ReverseMap();
                CreateMap<InvoiceItemDto, InvoiceItem>().ReverseMap();
            }
        
    }
}
