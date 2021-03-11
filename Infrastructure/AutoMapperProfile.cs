using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LaundryApi.Entites;
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
            CreateMap<NewCustomerDto, Customer>().ReverseMap();
            CreateMap<Customer, Customer>();

            CreateMap<NewEmployeeDto, Employee>().ReverseMap();
            CreateMap<EmployeeDtoPartial, Employee>().ReverseMap();
            CreateMap<EmployeeDto, Employee>().ReverseMap();

            CreateMap<ServiceDto, Service>().ReverseMap();
            CreateMap<ServiceDtoPartial, Service>().ReverseMap();

            CreateMap<NewInvoiceDto, Invoice>().ReverseMap();
            CreateMap<InvoiceDto, Invoice>().ReverseMap();



            CreateMap<InvoiceItemDto, InvoiceItem>().ReverseMap();
            CreateMap<NewInvoiceItemDto, InvoiceItem>();

            
        }
        
    }
}
