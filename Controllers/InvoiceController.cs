using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LaundryApi.Dtos;
using LaundryApi.Models;
using AutoMapper;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class InvoiceController : Controller
    {

        IInvoiceDbService dbService;
        
        public InvoiceController(IInvoiceDbService dbService)
        {
            this.dbService = dbService;
        }

        //GET: api/Invoice/{}

        //POST: api/Invoice/add
        [HttpPost("add")]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            invoiceDto=await dbService.AddInvoice(invoiceDto);
            return CreatedAtAction("", new { invoiceId=invoiceDto.InvoiceId}, invoiceDto);
        }
    }
}
