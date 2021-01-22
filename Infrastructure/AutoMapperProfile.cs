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

                CreateMap<ApplicationUser, LaundryDto>().ReverseMap();
                CreateMap<NewLaundryDto,ApplicationUser>();

                CreateMap<CustomerDto, Customer>().ReverseMap();
                CreateMap<NewLaundryDto, Laundry>();
                CreateMap<LaundryDto, Laundry>().ReverseMap();
                CreateMap<Laundry, Laundry>();
                CreateMap<NewInvoiceDto, Invoice>().ReverseMap();
                CreateMap<InvoiceDto, Invoice>().ReverseMap();
                CreateMap<InvoiceItemDto, InvoiceItem>().ReverseMap();
                CreateMap<ServiceDto, Service>().ReverseMap();
                CreateMap<Service, Service>();
            }
        
    }
}
