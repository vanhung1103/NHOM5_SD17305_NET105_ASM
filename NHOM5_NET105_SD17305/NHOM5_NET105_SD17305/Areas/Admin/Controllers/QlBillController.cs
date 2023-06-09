﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using NHOM5_NET105_SD17305.Data.IServices;
using NHOM5_NET105_SD17305.Data.Models;
using NHOM5_NET105_SD17305.Data.Services;
using System.Net.WebSockets;

namespace NHOM5_NET105_SD17305.Views.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QlBillController : Controller
    {
        private readonly IBillServices _billService;
        public INotyfService _notyfService { get; }
        public QlBillController(IBillServices billServices, INotyfService notyfService)
        {
            _billService = billServices;
            _notyfService = notyfService;
        }
        public async Task<IActionResult> Index(int status)
        {
           
            var a = await _billService.GetAllBillAsync();
            var b = a.FindAll(c => c.BillStatus_Id == status);
            return View(b);
        }
		public async Task<IActionResult> UpdateStatus(int status, int idbill)
		{
			var bill = await _billService.GetBillByIdAsync(idbill);
			if (bill == null)
			{
				return NotFound(); // Handle the case when the bill with the given id is not found
			}

			bill.BillStatus_Id = status;
			await _billService.UpdateBillAsync(bill);

			return RedirectToAction("Index");
		}


		[HttpGet]
        public async Task<IActionResult> UpdateBill(int id) // Mở form, truyền luôn sang form
        {
            var role = await _billService.GetBillByIdAsync(id);
            ViewBag.bill = role;
            return View(role);
        }
        public async Task<IActionResult> UpdateBill(Bill r) // Mở form
        {
            if (await _billService.UpdateBillAsync(r))
            {
                _notyfService.Success("Cập nhật thành công");
                return RedirectToAction("Index", "QlBill", new { area = "Admin" });
            }
            else
            {
                _notyfService.Error("Có lỗi xẩy ra");
                return BadRequest();
            }
        }
        public async Task<IActionResult> DeleteBill(int id)
        {
           await _billService.DeleteBillAsync(id);
            _notyfService.Success("Xóa thành công");
            return RedirectToAction("Index", "QlBill", new { area = "Admin" });
        }
        [HttpGet]
        public async Task<IActionResult> DetailsBill(int id)
        {
            var Role = await _billService.GetBillByIdAsync(id);
            return View(Role);
        }


    }
}
