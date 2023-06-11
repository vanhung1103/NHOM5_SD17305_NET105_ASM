﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NHOM5_NET105_SD17305.Data.IServices;
using NHOM5_NET105_SD17305.Data.Models;
using NHOM5_NET105_SD17305.Data.Services;

namespace NHOM5_NET105_SD17305.Views.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class CheckoutController : Controller
	{
		private readonly IBillStatusServices _billStatusService;
		private readonly IVnPayService _vnPayService;
		private readonly ICartServices _cartService;
		private readonly IcartItemServices _cartItemServices;
		private readonly IProductServices _productServices;
		private readonly ICombosServices _combosServices;
		private readonly IBillServices _billServices;
		private readonly IBillItemServices _billItemServices;
		public INotyfService _notyfService { get; }
		public int _UserID;
		public static int _BillID;
		public CheckoutController(IVnPayService vnPayService, ICartServices cartServices, IcartItemServices cartItemServices, IProductServices productServices, ICombosServices combosServices, IBillServices billServices, IBillStatusServices billStatusServices, IBillItemServices billItemServices, INotyfService notyfService)
		{
			_billStatusService = billStatusServices;
			_vnPayService = vnPayService;
			_cartService = cartServices;
			_cartItemServices = cartItemServices;
			_productServices = productServices;
			_combosServices = combosServices;
			_billItemServices = billItemServices;
			_billServices = billServices;
			_notyfService = notyfService;
		}

		public async Task<IActionResult> IndexAsync()
		{
			_UserID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

			var cart = await _cartService.GetAllCartAsync();
			int cartID = cart.FirstOrDefault(c => c.UserId == _UserID).Id;

			var CartItem = await _cartItemServices.GetAllCartItemAsync();
			var cartItems = CartItem.Where(c => c.CartId == cartID);
			int total = 0;
			foreach (var item in cartItems)
			{
				total += item.Price;

			}
			ViewBag.total = total;
			return View();
		}

		public async Task<IActionResult> BillssAsync()
		{
			_UserID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
			List<Bill> billuser = await _billServices.GetAllBillAsync();
			var bill = billuser.FirstOrDefault(c => c.UserId == _UserID && c.Id == _BillID);
			return View(bill);
		}

		public async Task<IActionResult> PaymentOff()
		{
			_UserID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

			var cart = await _cartService.GetAllCartAsync();
			int cartID = cart.FirstOrDefault(c => c.UserId == _UserID).Id;

			var CartItem = await _cartItemServices.GetAllCartItemAsync();
			var cartItems = CartItem.Where(c => c.CartId == cartID);
			int total = 0;
			foreach (var item in cartItems)
			{
				total += item.Price;

			}
			ViewBag.total = total;
			return View();
		}


		public async Task<IActionResult> PaymentOfAsync(Bill bill)
		{
			_UserID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

			var cart = await _cartService.GetAllCartAsync();
			int cartID = cart.FirstOrDefault(c => c.UserId == _UserID).Id;

			var CartItem = await _cartItemServices.GetAllCartItemAsync();
			var cartItems = CartItem.Where(c => c.CartId == cartID);
			int total = 0;
			foreach (var item in cartItems)
			{
				total += item.Price;
			}
			ViewBag.total = total;

			var bills = new Bill
			{
				UserId = _UserID,
				BillStatus_Id = bill.BillStatus_Id,
				Payment_Type_Id = bill.Payment_Type_Id,
				RecipientName = bill.RecipientName,
				RecipientPhone = bill.RecipientPhone,
				RecipientAddress = bill.RecipientAddress,
				Create_Date = DateTime.Now,
				Payment_date = DateTime.Now,
				Delivery_date = DateTime.Now,
				Discount = 1,
				ShippingFee = "0k",
				TotalAmount = total,
				Description = bill.Description
			};
			await _billServices.CreateBillAsync(bills);
			_BillID = bills.Id;
			List<BillItem> billItems = new List<BillItem>();
			List<Product> products = await _productServices.GetAllProductAsync();
			List<Combos> combo = await _combosServices.GetAllCombosAsync();
			foreach (var item in cartItems)
			{
				var billitem = new BillItem
				{
					Bill_Id = bills.Id,
					ProductId = item.ProductId,
					CombosId = item.CombosId,
					Quantity = item.Quantity,
					Price = item.Price
				};
				billItems.Add(billitem);

                var product = products.FirstOrDefault(c => c.Id == item.ProductId);
                var combos = combo.FirstOrDefault(c => c.Id == item.CombosId);
    //            if (item.Combos.Id != null)
				//{
				//	 combos.
				//}

				//var product = products.FirstOrDefault(c => c.Id == item.ProductId);
				//var combos = combo.FirstOrDefault(c => c.Id == item.CombosId);


				if(combos == null)
				{
					
					if (product.Quantity < billitem.Quantity)
					{
						_notyfService.Error("So luong san pham khong du");
						return RedirectToAction("GetAll", "Cart", new { area = "Customer" });
					}


				}
				else
				{
                    product.Quantity -= billitem.Quantity;
                    await _productServices.UpdateProductAsync(product);
                }

				if(product == null)
				{
					if (combos.Quantity < item.Quantity)
					{
                        
                        _notyfService.Error("So luong com bo khong du");
						return RedirectToAction("GetAll", "Cart", new { area = "Customer" });
					}
					
				}
                else
                {
                    combos.Quantity -= billitem.Quantity;
                    await _combosServices.UpdateCombosAsync(combos);
                }

            }


			var billstatus = await _billStatusService.GetAllBillStatusAsync();
			var check = billstatus.FirstOrDefault(c => c.Id == bills.BillStatus_Id);

			foreach (var billItem in billItems)
			{
				await _billItemServices.CreateBillItemAsync(billItem);
			}
			return RedirectToAction("Billss", "Checkout", new { area = "Customer" });
		}



		public IActionResult Payments()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Payments(string pm)
		{
			if (pm == "true")
			{
				return RedirectToAction("PaymentOff", "Checkout", new { area = "Customer" });
			}
			if (pm == "false")
			{

				return RedirectToAction("Index", "Checkout", new { area = "Customer" });
			}
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreatePaymentUrl(PaymentInformationModel model, Bill bill)
		{
			try
			{
				_UserID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

				var cart = await _cartService.GetAllCartAsync();
				int cartID = cart.FirstOrDefault(c => c.UserId == _UserID).Id;

				var CartItem = await _cartItemServices.GetAllCartItemAsync();
				var cartItems = CartItem.Where(c => c.CartId == cartID);
				int total = 0;
				foreach (var item in cartItems)
				{
					total += item.Price;
				}
				ViewBag.total = total;

				var bills = new Bill
				{
					UserId = _UserID,
					BillStatus_Id = bill.BillStatus_Id,
					Payment_Type_Id = bill.Payment_Type_Id,
					RecipientName = bill.RecipientName,
					RecipientPhone = bill.RecipientPhone,
					RecipientAddress = bill.RecipientAddress,
					Create_Date = DateTime.Now,
					Payment_date = DateTime.Now,
					Delivery_date = DateTime.Now,
					Discount = 1,
					ShippingFee = "0k",
					TotalAmount = total,
					Description = "ok"
				};
				await _billServices.CreateBillAsync(bills);


				ViewBag.idbill = bills.Id;
				List<BillItem> billItems = new List<BillItem>();
				foreach (var item in cartItems)
				{
					var billitem = new BillItem
					{
						Bill_Id = bills.Id,
						ProductId = item.ProductId,
						CombosId = item.CombosId,
						Quantity = item.Quantity,
						Price = item.Price
					};
					billItems.Add(billitem);
				}


				var billstatus = await _billStatusService.GetAllBillStatusAsync();
				var check = billstatus.FirstOrDefault(c => c.Id == bills.BillStatus_Id);

				foreach (var billItem in billItems)
				{
					await _billItemServices.CreateBillItemAsync(billItem);
				}
				var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
				return Redirect(url);
			}
			catch (Exception ex)
			{
				// Xử lý lỗi tạo Bill
				return RedirectToAction("Error", "Checkout");
			}
		}


		public IActionResult PaymentCallback()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			if (response.VnPayResponseCode == "00")
			{
				return RedirectToAction("Billss", "Checkout", new { area = "Customer" });
			}
			else
			{

				return RedirectToAction("Payments", "Checkout", new { area = "Customer" });
			}
		}

	}
}