//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using LaundryApi.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using LaundryApi.Dtos;
//using LaundryApi.Models;
//using AutoMapper;
//using static LaundryApi.Infrastructure.HelperMethods;

//namespace LaundryApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]

//    public class InvoiceController : ControllerBase
//    {

//        private readonly IInvoiceRepository invoiceRepository;
        

//        public InvoiceController(IInvoiceRepository invoiceRepository)
//        {
//            this.invoiceRepository = invoiceRepository;
//        }

//        //GET: api/Invoice/{invoiceId}
//        [HttpGet("{invoiceId}")]
//        public async Task<ActionResult<InvoiceDto>> ReadInvoice(Guid invoiceId)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest();
//            try
//            {
//                var invoice = await invoiceRepository.ReadInvoice(invoiceId);
//                return Ok(invoice);
//            }
//            catch (Exception e)
//            {
//                if (e.Message == ErrorMessage.EntityDoesNotExist)
//                    return BadRequest();

//                //IF you got to this point an unforseen error occurred
//                return StatusCode(500);
//            }
//        }

//        //GET: api/Invoice
//        [HttpGet]
//        public ActionResult<IEnumerable<InvoiceDto>> GetInvoices()
//        {
//            try
//            {
//                var returnObj = invoiceRepository.GetInvoices();
//                return Ok(returnObj);
//            }
//            catch
//            {
//                return StatusCode(500);
//            }
            
//        }

//        //POST: api/Invoice/add
//        [HttpPost("new")]
//        public ActionResult<NewInvoiceDto> CreateInvoice(NewInvoiceDto newInvoiceDto)
//        {
             

//            if (!ModelState.IsValid)
//                return BadRequest();
//            try
//            {
//                InvoiceDto invoiceDto = invoiceRepository.AddInvoice(newInvoiceDto, HttpContext.User.Identity.Name);
//                return CreatedAtAction(nameof(ReadInvoice), new { invoiceId = invoiceDto.InvoiceId }, invoiceDto);
//            }
//            catch(Exception e)
//            {
//                if (e.Message == ErrorMessage.EntityDoesNotExist)
//                    return BadRequest();

//                //if you got this point some unforseen error occured
//                return StatusCode(500);

//            }
            
//        }
//    }
//}
