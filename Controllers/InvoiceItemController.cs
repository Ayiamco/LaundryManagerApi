using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using LaundryApi.Dtos;
using static LaundryApi.Services.HelperMethods;

namespace LaundryApi.Controllers
{
    public class InvoiceItemController : Controller
    {
        private readonly IInvoiceItemRepository dbService;

        public InvoiceItemController(IInvoiceItemRepository  dbService)
        {
            this.dbService = dbService;
        }

        //POST: api/InvoiceItem
        public ActionResult<InvoiceDto> CreateInvoiceItem(List<InvoiceItemDto> invoiceDtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                dbService.AddInvoiceItem(invoiceDtoList);
                return StatusCode(201);
            }
            catch
            {
              return StatusCode(500);
            }
        }
    }
}
