using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) return NotFound();

            var modelList = new List<OrderDetailsViewModel>();

            var orderHeaderList = await _db.OrderHeader.Include(o => o.User).Where(o => o.UserId.Equals(claim.Value)).ToListAsync();

            //orderHeaderList.ForEach(async item =>
            //    {
            //        modelList.Add(new OrderDetailsViewModel
            //        {
            //            OrderHeader = item,
            //            OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(item.Id.ToString())).ToListAsync()
            //        });
            //    });

            foreach (var item in orderHeaderList)
            {
                modelList.Add(new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(item.Id.ToString())).ToListAsync()
                });
            }


            return View(modelList);
        }

        [Authorize]
        public async Task<IActionResult> Confirm(Guid id) 
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderHeader = await _db.OrderHeader.Include(o => o.User).FirstOrDefaultAsync(o => o.Id.ToString().Equals(id.ToString())),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(id.ToString())).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }

        public async Task<IActionResult> GetOrderDetails(Guid Id)
        {
            if (Id == null) return NotFound();

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderHeader = await _db.OrderHeader.FirstOrDefaultAsync(o => o.Id.ToString().Equals(Id.ToString())),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(Id.ToString())).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.User = await _db.ApplicationUser.FirstOrDefaultAsync(u=>u.Id.Equals(orderDetailsViewModel.OrderHeader.UserId));

            return PartialView("_InvidualOrderDetailsPartial",orderDetailsViewModel);
        }

    }
}
