﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LaundryApi.Dtos;
using LaundryApi.Entites;
using AutoMapper;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Infrastructure;
using LaundryApi.Models;

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

        //GET: api/invoice?pageindex={int}&pagesize={int}
        [HttpGet]
        public ActionResult Index(int pageSize,int pageIndex)
        {
            string role = HttpContext.GetUserRole();
            if ( !(role==RoleNames.LaundryOwner || role == RoleNames.LaundryEmployee))
            {
                return StatusCode(401);
            }
            IEnumerable<InvoiceDto> invoices=invoiceRepository.GetInvoices(pageIndex,pageSize,HttpContext.User.Identity.Name,role);

            return Ok(new ResponseDto<IEnumerable<InvoiceDto>>() { data=invoices,statusCode="200"});
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

        //[HttpGet()]
        //public ActionResult MakePayment()
        //{
        //    return Ok();
        //}


        //GET: api/invoice/items/{id}
        [HttpGet("invoiceItems/{invoiceId}")]
        public async Task<ActionResult<InvoiceDto>> ReadInvoiceItems(Guid invoiceId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResponseDto<InvoiceDto>() { message = ErrorMessage.InvalidModel });

                //get the full invoice including all associated invoice items
                var invoiceDto = await invoiceRepository.ReadCompleteInvoiceAsync(invoiceId);

                //return value
                return Ok(invoiceDto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(new ResponseDto<InvoiceDto>() { message = ErrorMessage.EntityDoesNotExist });

                // if you get to this point something unforseen happened
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
