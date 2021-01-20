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

    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceRepository dbService;

        public InvoiceController(IInvoiceRepository dbService)
        {
            this.dbService = dbService;
        }

        //GET: api/Invoice/{invoiceId}
        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<InvoiceDto>> ReadInvoice(Guid invoiceId)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var invoice=await dbService.ReadInvoice(invoiceId);
            return Ok(invoice);
        }

        //GET: api/Invoice
        [HttpGet]
        public ActionResult<IEnumerable<InvoiceDto>> GetInvoices()
        {
            var returnObj = dbService.GetInvoices();
            return Ok(returnObj);
        }

        //POST: api/Invoice/add
        [HttpPost("add")]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            invoiceDto=await dbService.AddInvoice(invoiceDto);
            return CreatedAtAction(nameof(ReadInvoice), new { invoiceId=invoiceDto.InvoiceId}, invoiceDto);
        }
    }
}
