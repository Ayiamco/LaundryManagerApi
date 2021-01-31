using LaundryApi.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LaundryApi.Infrastructure
{
   
    public  class MailService:IMailService
    {
        readonly IConfiguration config;

        public MailService(IConfiguration config)
        {
            this.config = config;
        }
        public  async Task SendMailAsync(string recieverEmail, string messageBody, string messageSubject)
        {
            try
            {
                
                string username = config["Movies:emailUsername"];
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = config["Movies:emailUsername"],
                    Password = config["Movies:emailPassword"]
                };
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mailMessage = new MailMessage()
                {
                    To = { recieverEmail },
                    Body = messageBody,
                    From = new MailAddress("ayiamco@gmail.com"),
                    Subject = messageSubject,
                    IsBodyHtml=true
                };

                await smtpClient.SendMailAsync(mailMessage);
                smtpClient.Dispose();
                return;
            }
            catch (Exception e) { Console.WriteLine(e); }


        }



    }
}
