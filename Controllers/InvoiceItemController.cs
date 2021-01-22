//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using LaundryApi.Interfaces;
//using LaundryApi.Models;
//using LaundryApi.Dtos;
//using static LaundryApi.Infrastructure.HelperMethods;

//namespace LaundryApi.Controllers
//{
//    public class InvoiceItemController : Controller
//    {
//        private readonly IInvoiceItemRepository invoiceItemRepository;

//        public InvoiceItemController(IInvoiceItemRepository invoiceItemRepository)
//        {
//            this.invoiceItemRepository = invoiceItemRepository;
//        }

//        //POST: api/InvoiceItem
//        public ActionResult<NewInvoiceDto> CreateInvoiceItem(List<InvoiceItemDto> invoiceItemDtoList)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest();

//            try
//            {
//                invoiceItemRepository.AddInvoiceItem(invoiceItemDtoList);
//                return StatusCode(201);
//            }
//            catch
//            {
//              return StatusCode(500);
//            }
//        }
//    }
//}
