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

            CreateMap<Laundry, LaundryDto>().ReverseMap();
            CreateMap<NewLaundryDto,Laundry>();  
            CreateMap<CustomerDto, Customer>().ReverseMap();

            CreateMap<NewEmployeeDto, Employee>().ReverseMap();
            CreateMap<EmployeeDto, Employee>().ReverseMap();

                //CreateMap<NewInvoiceDto, Invoice>().ReverseMap();
                //CreateMap<InvoiceDto, Invoice>().ReverseMap();
                //CreateMap<InvoiceItemDto, InvoiceItem>().ReverseMap();
                //CreateMap<ServiceDto, Service>().ReverseMap();
                //CreateMap<Service, Service>();
            }
        
    }
}
