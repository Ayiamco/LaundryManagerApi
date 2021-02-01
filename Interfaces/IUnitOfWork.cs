using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IUnitOfWork
    {
        public ICustomerRepository CustomerRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IInvoiceRepository InvoiceRepository { get; }
        public  ILaundryRepository LaundryRepository { get; }
        public IEmployeeRepository EmployeeRepository { get; }
        public IRepositoryHelper RepositoryHelper { get; }
        public IMailService MailService { get; }
        public IMapper Mapper { get; }
        public IJwtAuthenticationManager JwtAuthenticationManager { get; }
        public IManagerRepository ManagerRepository { get; }

    }
}
