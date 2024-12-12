using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using Bulky.Models.View_Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("admin")]
    [Authorize]
	public class OrderController : Controller
	{
        private readonly IUnitofWork unitofWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitofWork unitofWork)
		{
            this.unitofWork = unitofWork;
        }
		public IActionResult Index()
		{
			return View();
		}

        
     
        public IActionResult Details( int orderId)
        {
             orderVM = new()
            {
                OrderHeader = unitofWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = unitofWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }

       
        [Authorize(Roles = SD.Role_Employee+","+SD.Role_Admin)]
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDb = unitofWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name= orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.TrackingNumber;
            }
            unitofWork.OrderHeader.update(orderHeaderFromDb);
            unitofWork.Save();

            TempData["success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new {orderId= orderHeaderFromDb.Id});


        }



        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        [HttpPost]
        public IActionResult StartProcessing()
        {
            unitofWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            unitofWork.Save();
            TempData["success"] = "Order Details Updated Sucessfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
        [HttpPost]
        public IActionResult ShipOrder()
        {
           
            var orderHeader = unitofWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueData = DateTime.Now.AddDays(30);
            }

            unitofWork.OrderHeader.update(orderHeader);
            unitofWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
 
        }


       
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {

            var orderHeader = unitofWork.OrderHeader.Get(u => u.Id == orderVM.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                unitofWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                unitofWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            unitofWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

        }


        

        [HttpPost]

        [ActionName(nameof(Details))]
        public IActionResult Details_PAY_NOW()
        { 
           
            orderVM.OrderHeader = unitofWork.OrderHeader
           .Get(u => u.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderVM.OrderDetails = unitofWork.OrderDetail
                .GetAll(u => u.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "Product");


          
            //var domain = "https://localhost:44383/";
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {

                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

           
            foreach (var item in orderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
              
                options.LineItems.Add(sessionLineItem);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            
            unitofWork.OrderHeader.UpdateStripPaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            unitofWork.Save();
            
            Response.Headers.Add("Location", session.Url);
            
            return new StatusCodeResult(303);
        }

     
        public IActionResult PaymentConfirmation( int orderHeaderId)
        {
            OrderHeader orderHeader = unitofWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitofWork.OrderHeader.UpdateStripPaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                                                                       
                    unitofWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    unitofWork.Save();
                }
            }

           
            return View(orderHeaderId);
        }


       
        #region API CALL
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders = unitofWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();


            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = unitofWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = unitofWork.OrderHeader
                    .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

        

            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

                    return Json(new { data = objOrderHeaders });
		}

		#endregion
	}
}
