using LaundryApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Infrastructure;
using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Models;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly LaundryApiContext _context;
        private readonly IMapper mapper;
        public ServiceRepository(LaundryApiContext _context, IMapper mapper)
        {
            this.mapper = mapper;
            this._context = _context;
        }

        public  ServiceDto CreateService(ServiceDto serviceDto, string username)
        {
            try
            {
                var applicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Username == username);
                if (applicationUser == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);
                serviceDto.ApplicationUserId = applicationUser.Id;
                Service service = mapper.Map<Service>(serviceDto);
                service.CreatedAt = DateTime.Now;
                service.UpdatedAt = DateTime.Now;
                 _context.Services.Add(service);
                 _context.SaveChanges();

                serviceDto.Id = service.Id;
                return serviceDto;
            }
            catch(Exception e)
            {
                string errorMessage = e.InnerException.ToString();
                if (errorMessage.Contains("Violation of PRIMARY KEY constraint 'PK_Services'"))
                    throw new Exception(ErrorMessage.ServiceAlreadyExist);
                    
                throw new Exception(ErrorMessage.FailedDbOperation);
            }

        }

        public void UpdateService(ServiceDto serviceDto)
        {
            try
            {

                var serviceInDb = _context.Services.SingleOrDefault(s => s.Id == serviceDto.Id);
                if (serviceInDb == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                serviceInDb.UpdatedAt = DateTime.Now;
                serviceInDb.Description = serviceDto.Description;
                serviceInDb.Price = serviceDto.Price;
                _context.SaveChanges();
                return;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }
        }

        public void DeleteService(Guid serviceId)
        {
            try
            {
                var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
                if (service == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);
                _context.Remove(service);
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public ServiceDto GetService(Guid id)
        {
            try
            {
                var service = _context.Services.FirstOrDefault(s => s.Id == id);
                if (service == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                var serviceDto = mapper.Map<ServiceDto>(service);
                return serviceDto;
            }
            catch
            {
                throw new Exception(ErrorMessage.EntityDoesNotExist);
            }


        }

        public IEnumerable<ServiceDto> GetAllLaundryServices()
        {
            try
            {

                var laundryServices = _context.Services.ToList();
                List<ServiceDto> serviceDtos = new List<ServiceDto>();
                laundryServices.ForEach(service =>
                {
                    var dto = mapper.Map<ServiceDto>(service);
                    serviceDtos.Add(dto);
                });
                return serviceDtos;
            }
            catch
            {
                throw new Exception(ErrorMessage.FailedDbOperation);
            }


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
