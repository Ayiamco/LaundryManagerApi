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
        public void UpdateService(ServiceDto servicedto);
        public void DeleteService(Guid serviceid);
        public ServiceDto GetService(Guid id);

        //public IEnumerable<ServiceDto> GetAllLaundryServices();
    }
}
