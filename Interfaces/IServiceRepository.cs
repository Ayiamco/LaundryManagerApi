using LaundryApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IServiceRepository
    {
        public Task<ServiceDto> AddService(ServiceDto serviceDto,string laundryUsername);
        public void UpdateService(ServiceDto serviceDto);
        public void DeleteService(Guid serviceId);
        public ServiceDto GetService(Guid id);
        public IEnumerable<ServiceDto> GetAllLaundryServices();
    }
}
