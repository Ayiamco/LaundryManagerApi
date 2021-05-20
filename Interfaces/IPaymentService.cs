using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IPaymentService
    {

        Task<string> InitiazlizePayment();
    }
}