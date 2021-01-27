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
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Infrastructure;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceRepository invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;

        }

        //GET: api/Invoice/{invoiceId}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> ReadInvoice(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var invoice = await invoiceRepository.ReadInvoice(id);
                return Ok(invoice);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest();

                //IF you got to this point an unforseen error occurred
                return StatusCode(500);
            }
        }

        ////GET: api/invoice/items/{id}
        //[HttpGet("/items/{id}")]
        //public async Task<ActionResult<InvoiceDto>> ReadInvoiceItems(Guid id)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(new ResponseDto<InvoiceDto>() { message = ErrorMessage.InvalidModel });

        //        //get the full invoice including all associated invoice items
        //        var invoiceDto = await invoiceRepository.ReadCompleteInvoiceAsync(id);

        //        //return value
        //        return Ok(invoiceDto);
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.Message == ErrorMessage.EntityDoesNotExist)
        //            return BadRequest(new ResponseDto<InvoiceDto>() { message = ErrorMessage.EntityDoesNotExist });

        //        // if you get to this point something unforseen happened
        //        return StatusCode(500);
        //    }


        //}

        //GET: api/Invoice
        [HttpGet]
        public ActionResult<IEnumerable<InvoiceDto>> GetInvoices()
        {
            try
            {
                var returnObj = invoiceRepository.GetInvoices();
                return Ok(returnObj);
            }
            catch
            {
                return StatusCode(500);
            }

        }

        //POST: api/Invoice/add
        [HttpPost("new")]
        public ActionResult<NewInvoiceDto> CreateInvoice(NewInvoiceDto newInvoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto<NewInvoiceDto>() { message = ErrorMessage.InvalidModel });

            try
            {
                string role = HttpContext.GetUserRole();
                if ( !(role==RoleNames.LaundryEmployee || role==RoleNames.LaundryOwner))
                    return Unauthorized();

                InvoiceDto invoiceDto = invoiceRepository.AddInvoice(newInvoiceDto,role,HttpContext.User.Identity.Name);
                return CreatedAtAction(nameof(ReadInvoice), new { id = invoiceDto.Id }, invoiceDto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(new ResponseDto<InvoiceDto>() { message = "Customer does not exist" });

                else if (e.Message == ErrorMessage.InvalidToken)
                    return BadRequest(new ResponseDto<InvoiceDto>() { message = "Customer was is not in users laundry" });

                //if you got this point some unforseen error occured
                return StatusCode(500);

            }

        }
    }
}
