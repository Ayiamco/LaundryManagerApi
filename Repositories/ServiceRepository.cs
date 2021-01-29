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
                string errorMessage = e.InnerException.ToString();
                if (errorMessage.Contains("Violation of UNIQUE KEY constraint 'AK_Services_Description_LaundryId'"))
                    throw new Exception(ErrorMessage.ServiceAlreadyExist);

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
                serviceInDb.Description = serviceDto.Description;
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

                if (service == null)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                var serviceDto = mapper.Map<ServiceDto>(service);
                return serviceDto;
            }
            catch(Exception e)
            {
                if(e.Message==ErrorMessage.EntityDoesNotExist)
                    throw new Exception(ErrorMessage.EntityDoesNotExist);

                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        public IEnumerable<ServiceDtoPartial> GetMyLaundryServices(string username,string userRole)
        {
            try
            {
                IEnumerable<ServiceDtoPartial> serviceDto;
                ApplicationUser user=repositoryHelper.GetApplicationUser(username);
                if(userRole ==RoleNames.LaundryEmployee)
                    serviceDto=GetServicesForLaundryOwner(repositoryHelper.GetLaundryByUsername(username));
                
                else
                    serviceDto = GetServicesForEmployee(repositoryHelper.GetEmployeeByUsername(username));

                return serviceDto;
            }

            catch(Exception e)
            {
                if (e.Message == ErrorMessage.NoEntityMatchesSearch)
                    throw new Exception(ErrorMessage.NoEntityMatchesSearch);
                throw new Exception(ErrorMessage.FailedDbOperation);
            }


        }

        private IEnumerable<ServiceDtoPartial> GetServicesForLaundryOwner(Laundry user)
        {
            var laundryServices = _context.Services.Where(x => x.LaundryId == user.Id).ToList();
            if (laundryServices.Count == 0)
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);
            List<ServiceDtoPartial> serviceDtos = new List<ServiceDtoPartial>();
            IEnumerable<ServiceDtoPartial> serviceDto = mapper.Map<IEnumerable<ServiceDtoPartial>>(laundryServices);

            return serviceDto;
        }

        private IEnumerable<ServiceDtoPartial> GetServicesForEmployee(Employee user)
        {
            var laundryServices = _context.Services.Where(x => x.LaundryId == user.LaundryId).ToList();
            if (laundryServices.Count == 0)
                throw new Exception(ErrorMessage.NoEntityMatchesSearch);

            List<ServiceDtoPartial> serviceDtos = new List<ServiceDtoPartial>();
            foreach(Service service  in laundryServices)
            {
                var dto=mapper.Map<ServiceDtoPartial>(service);
                dto.Revenue = 0;
                serviceDtos.Add(dto);
            }

            return serviceDtos;
            
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
