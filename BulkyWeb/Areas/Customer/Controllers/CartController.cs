using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork unitofWork;
        private readonly IEmailSender _emailSender;
       

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitofWork unitofWork, IEmailSender emailSender)
        {
            this.unitofWork = unitofWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            ShoppingCartVM = new()
            {
                shoppingCartList = unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                orderHeader=new()
                
            };


      
            IEnumerable<ProductImage> productImages = unitofWork.ProductImage.GetAll();

       
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
              
                cart.Price = GetPriceBaseOnQuantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }



        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           
            ShoppingCartVM = new()
            {
                shoppingCartList = unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                orderHeader = new()

            };

            

            ShoppingCartVM.orderHeader.ApplicationUser =unitofWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
            ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
            ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;

           
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
              
                cart.Price = GetPriceBaseOnQuantity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);

        }


        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
            
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            


            ShoppingCartVM.shoppingCartList = unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            
            ShoppingCartVM.orderHeader.OrderDate =System.DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = unitofWork.ApplicationUser.Get(u => u.Id == userId);

		
			foreach (var cart in ShoppingCartVM.shoppingCartList)
			{
				
				cart.Price = GetPriceBaseOnQuantity(cart);
				ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
			}


			
		
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
			}
            else
            {
				ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
			}
            
            unitofWork.OrderHeader.Add(ShoppingCartVM.orderHeader);
            unitofWork.Save();
            
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                { 
                    ProductId = cart.ProductId,
                   
                    OrderHeaderId =ShoppingCartVM.orderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                unitofWork.OrderDetail.Add(orderDetail);
                unitofWork.Save();
            }

			
		
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
              
                //var domain = "https://localhost:44383/";
                
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                


                var options = new SessionCreateOptions
				{
                    
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.orderHeader.Id}",
					CancelUrl = domain + "customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

              
				foreach (var item in ShoppingCartVM.shoppingCartList)
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
				 
				unitofWork.OrderHeader.UpdateStripPaymentID(ShoppingCartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
				unitofWork.Save();
				
				Response.Headers.Add("Location", session.Url);
				
				return new StatusCodeResult(303);
			}

            
			return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.orderHeader.Id});

		}




        public IActionResult OrderConfirmation(int id)
        {
			OrderHeader orderHeader = unitofWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitofWork.OrderHeader.UpdateStripPaymentID(id, session.Id, session.PaymentIntentId);
                    unitofWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    unitofWork.Save();
                }

  
                HttpContext.Session.Clear();
            }

            

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
              $"<p>New Order Created - {orderHeader.Id}</p>");

            

            List<ShoppingCart> shoppingCarts = unitofWork.ShoppingCart
			 .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			unitofWork.ShoppingCart.RemoveRange(shoppingCarts);
			unitofWork.Save();
			return View(id);
        }



       
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = unitofWork.ShoppingCart.Get(u=>u.Id == cartId);
            cartFromDb.Count += 1;
            unitofWork.ShoppingCart.update(cartFromDb);
            unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }


       
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = unitofWork.ShoppingCart.Get(u => u.Id == cartId,tracked: true);
            if(cartFromDb.Count <= 1)
            {
                
                HttpContext.Session.SetInt32(SD.SessionCart, unitofWork.ShoppingCart
                  .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

                
                unitofWork.ShoppingCart.Remove(cartFromDb);

            }
            else
            {
                cartFromDb.Count -= 1;
                unitofWork.ShoppingCart.update(cartFromDb);
            }
            
            unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }



     
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = unitofWork.ShoppingCart.Get(u => u.Id == cartId ,tracked:true);
            
            HttpContext.Session.SetInt32(SD.SessionCart, unitofWork.ShoppingCart
              .GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            unitofWork.ShoppingCart.Remove(cartFromDb);
            unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }



     

        private double GetPriceBaseOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if(shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }
    }
}
