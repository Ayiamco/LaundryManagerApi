using AutoMapper;
using LaundryApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICustomerRepository CustomerRepository { get; }

        public IDashboardRepo DashboardRepo { get; }
        public IServiceRepository ServiceRepository { get; }

        public IInvoiceRepository InvoiceRepository { get; }

        public ILaundryRepository LaundryRepository { get; }

        public IEmployeeRepository EmployeeRepository { get; }

        public IRepositoryHelper RepositoryHelper { get; }

        public IMailService MailService { get; }

        public IMapper Mapper { get; }

        public IJwtAuthenticationManager JwtAuthenticationManager { get; }

        public IManagerRepository ManagerRepository { get; }

       

        public UnitOfWork(ICustomerRepository CustomerRepository,IManagerRepository ManagerRepository,
            IMapper Mapper, IMailService MailService, IRepositoryHelper RepositoryHelper,
            IServiceRepository ServiceRepository, IEmployeeRepository EmployeeRepository,
             ILaundryRepository LaundryRepository,IInvoiceRepository InvoiceRepository,
             IJwtAuthenticationManager JwtAuthenticationManager,
             IDashboardRepo DashboardRepo)
        {
            this.CustomerRepository = CustomerRepository;
            this.EmployeeRepository = EmployeeRepository;
            this.ManagerRepository = ManagerRepository;
            this.MailService = MailService;
            this.Mapper = Mapper;
            this.InvoiceRepository = InvoiceRepository;
            this.ServiceRepository = ServiceRepository;
            this.InvoiceRepository = InvoiceRepository;
            this.JwtAuthenticationManager = JwtAuthenticationManager;
            this.LaundryRepository = LaundryRepository;
            this.DashboardRepo = DashboardRepo;

        }
       
    }
}
