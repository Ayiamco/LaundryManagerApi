using LaundryApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Infrastructure;
using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Entites;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LaundryApi.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        private readonly IRepositoryHelper repositoryHelper;
        public ServiceRepository(LaundryApiContext _context, IMapper mapper,IRepositoryHelper repositoryHelper)

        {
            this.mapper = mapper;
            this._context = _context;
            this.repositoryHelper = repositoryHelper;
        }

        public async Task<ServiceDto> CreateServiceAsync(ServiceDto serviceDto, string username)
        {
            try
            {
                //get the laundry trying to add the service 
                var laundry = _context.Laundries.SingleOrDefault(x => x.Username == username);
                if (laundry == null)
                    throw new Exception(ErrorMessage.UserDoesNotExist);

                //create the service object from the service Dto
                Service service = mapper.Map<Service>(serviceDto);

                //update the missing properties in the object
                service.LaundryId = laundry.Id;
                service.CreatedAt = DateTime.Now;
                service.UpdatedAt = DateTime.Now;

                //add the service object to db context 
                await _context.Services.AddAsync(service);

                //save changes 
                await _context.SaveChangesAsync();

                //pdated and return created service object
                serviceDto.Id = service.Id;
                serviceDto.LaundryId = laundry.Id;
                serviceDto.Laundry = null;
                return serviceDto;

            }
            catch (Exception e)
            {
                if (e.InnerException !=null)
                {
                    string errorMessage = e.InnerException.ToString();
                    if (errorMessage.Contains("Violation of UNIQUE KEY constraint 'AK_Services_Name_LaundryId'"))
                        throw new Exception(ErrorMessage.ServiceAlreadyExist);
                }
                
                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public async Task<ServiceDto> UpdateService(ServiceDto serviceDto)
        {
            try
            {
                //get the laundry service and check if it is null
                var serviceInDb = await _context.Services.FindAsync( serviceDto.Id);
                if (serviceInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //update the service with the new changes
                serviceInDb.UpdatedAt = DateTime.Now;
                serviceInDb.Price = serviceDto.Price;

                //save changes
                await _context.SaveChangesAsync();
                serviceDto = mapper.Map<ServiceDto>(serviceInDb);
                serviceDto.Laundry = null;
                return serviceDto ;
            }
            catch(Exception e)
            {
               
                if(e.Message==ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

        public async Task<bool> DeleteService(Guid serviceId)
        {
            try
            {
                //get the laundry service and check if it is null
                var serviceInDb = await _context.Services.FindAsync(serviceId); 
                if (serviceInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                //tag the service ass deleted
                serviceInDb.IsDeleted = true;

                //save changes
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public async Task<ServiceDto> GetService(Guid id)
        {
            try
            {
                var service = await _context.Services.FindAsync( id);

                if (service == null || service.IsDeleted)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);
                
                var serviceDto = mapper.Map<ServiceDto>(service);
                var revenue=_context.InvoiceItems.Include(x => x.Service)
                    .Where(x => x.ServiceId == serviceDto.Id).Sum(x=> x.Quantity * x.Service.Price);
                serviceDto.Revenue = revenue;
                return serviceDto;
            }
            catch(Exception e)
            {
                if(e.Message==ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }


        public PagedList<ServiceDto> GetPage(int pageSize, string username, string userRole, int pageNumber = 1, string searchParam = "")
        {
            var user = repositoryHelper.GetApplicationUser(username);
            Guid laundryId;
            if (userRole == RoleNames.LaundryEmployee)
                laundryId = repositoryHelper.GetEmployeeByUsername(username).LaundryId;
            else
                laundryId = repositoryHelper.GetLaundryByUsername(username).Id;
            

            var serviceList = _context.Services.Where(x => x.IsDeleted == false && x.LaundryId == laundryId).ToList();
            if (searchParam != "")
            {
                serviceList = serviceList.Where(x => x.Name.Contains(searchParam.ToLower())).ToList();
                if (serviceList.Count() / pageSize <= pageNumber)
                    pageNumber = 1;
            }

            var page = serviceList.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            foreach (Service service in page)
            {
                service.Revenue = _context.InvoiceItems.Include(x => x.Service)
                     .Where(x => x.ServiceId == service.Id).Sum(x => x.Quantity * x.Service.Price);
            }
            var maxPage = serviceList.Count / (decimal)pageSize;
            PagedList<ServiceDto> obj = new PagedList<ServiceDto>()
            {
                Data = mapper.Map<IEnumerable<ServiceDto>>(page),
                PageIndex = pageNumber,
                PageSize = pageSize,
            };
            if (maxPage < 1)
                obj.MaxPageIndex = 1;
            else
            {
                int _num;
                int _val;
                try
                { _num = Convert.ToInt32(Convert.ToString(maxPage).Split(".")[1]); 
                    _val= Convert.ToInt32(Convert.ToString(maxPage).Split(".")[0]);
                }
                catch
                { _num = 0;_val = Convert.ToInt32(maxPage); }

                obj.MaxPageIndex = _num > 0 ? _val + 1 : Convert.ToInt32(maxPage);
            }
            return obj;
        }

        public IEnumerable<ServiceDto> GetAllServices(string username,string userRole)
        {
            Guid laundryId;
            if (userRole == RoleNames.LaundryOwner)
                laundryId = _context.Laundries.SingleOrDefault(x => x.Username == username).Id;
            else
                laundryId = _context.Employees.SingleOrDefault(x => x.Username == username).LaundryId;

            var services =_context.Services.Where(x => x.LaundryId == laundryId).ToList();
            foreach(Service service in services)
            {
                service.Revenue= _context.InvoiceItems.Include(x => x.Service)
                     .Where(x => x.ServiceId == service.Id).Sum(x => x.Quantity * x.Service.Price);
            }
            var dto=mapper.Map<IEnumerable<ServiceDto>>(services);

            return dto;
        }

        }
}




//using Microsoft.EntityFrameworkCore.Migrations;

//namespace LaundryApi.Migrations
//{
//    public partial class seedingApplicationUserRoles : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.Sql("INSERT INTO [dbo].[Roles] ([Id], [Name]) VALUES (N'dd17cb53-2416-439a-3593-08d8beeb7a6a', N'LaundryOwner')");
//            migrationBuilder.Sql("INSERT INTO[dbo].[Roles]([Id], [Name]) VALUES(N'61d47b86-34b2-42da-3594-08d8beeb7a6a', N'LaundryEmployee')");
//            migrationBuilder.Sql("INSERT INTO[dbo].[Roles] ([Id], [Name]) VALUES(N'c802c039-8f04-4258-3595-08d8beeb7a6a', N'Admin')");
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.Sql("DELETE * FROM [dbo].[Roles]");
//        }
//    }
//}
