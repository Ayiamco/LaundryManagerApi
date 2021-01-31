using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Interfaces
{
    public interface IMailService
    {
        public Task SendMailAsync(string recieverEmail, string messageBody, string messageSubject);
    }
}
