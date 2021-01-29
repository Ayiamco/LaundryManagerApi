using LaundryApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IServiceRepository
    {
        public Task<ServiceDto> CreateServiceAsync(ServiceDto servicedto, string username);
        public Task<ServiceDto> UpdateService(ServiceDto serviceDto);
        public Task<bool> DeleteService(Guid serviceid);
        public Task<ServiceDto> GetService(Guid id);
        public IEnumerable<ServiceDtoPartial> GetMyLaundryServices(string username, string userRole);
    }
}
