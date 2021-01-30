using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _db;
        private int PageSize = SD.PageSize;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Index(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) return NotFound();

            OrderListViewModel orderListViewModel = new OrderListViewModel
            {
                Orders = new List<OrderDetailsViewModel>(),
                //PagingInfo = new Models.PagingInfo()
            };
            //var modelList = new List<OrderDetailsViewModel>();

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
                orderListViewModel.Orders.Add(new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(item.Id.ToString())).ToListAsync()
                });
            }

            var count = orderListViewModel.Orders.Count;
            orderListViewModel.Orders = orderListViewModel.Orders.OrderByDescending(p=>p.OrderHeader.Id)
                .Skip((productPage-1)*PageSize)
                .Take(PageSize).ToList();
            orderListViewModel.PagingInfo = new Models.PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = "/Customer/Order/Index?productPage=:" //we'll replace ':' it the view, with the page number
            };

            return View(orderListViewModel);
        }
        
        [Authorize(Roles = (SD.KitchenUser + "," + SD.ManagerUser))]
        public async Task<IActionResult> ManageOrder()
        {

            IList<OrderDetailsViewModel> orderDetailsViewModelList = new List<OrderDetailsViewModel>();

            //OrderListViewModel orderListViewModel = new OrderListViewModel
            //{
            //    Orders = new List<OrderDetailsViewModel>(),
            //    //PagingInfo = new Models.PagingInfo()
            //};

            //var orderHeaderListTest = await _db.OrderHeader.Include(o => o.User).ToListAsync();
            //var orderHeaderListTest2 = await _db.OrderHeader.ToListAsync();
            var orderHeaderList = await _db.OrderHeader.Where(o => o.Status.Equals(SD.Status.orderSubmitted)|| o.Status.Equals(SD.Status.orderInProcess))
                .OrderByDescending(o=>o.PickupTime).ToListAsync();

            foreach (var item in orderHeaderList)
            {
                orderDetailsViewModelList.Add(new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(item.Id.ToString())).ToListAsync()
                });
            }

            return View(orderDetailsViewModelList);
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
            if (Id == null || Id.Equals(Guid.Empty)) return NotFound();

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
            {
                OrderHeader = await _db.OrderHeader.FirstAsync(o => o.Id.ToString().Equals(Id.ToString())),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(Id.ToString())).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.User = await _db.ApplicationUser.FirstAsync(u=>u.Id.Equals(orderDetailsViewModel.OrderHeader.UserId));

            return PartialView("_InvidualOrderDetailsPartial",orderDetailsViewModel);
        }

        public async Task<IActionResult> GetOrderStatusChart(string status) 
        {
            return PartialView("_OrderStatusChartPartialView",status);
        }

        [Authorize(Roles = (SD.KitchenUser + "," + SD.ManagerUser))]
        public async Task<IActionResult> OrderPrepare(Guid OrderId) 
        {
            if (OrderId == null || OrderId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.OrderHeader.FirstOrDefaultAsync(o => o.Id.ToString().Equals(OrderId.ToString()));
            if (result == null) return NotFound();

            result.Status = SD.Status.orderInProcess;
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder");
        }

        [Authorize(Roles = (SD.KitchenUser + "," + SD.ManagerUser))]
        public async Task<IActionResult> OrderCancel(Guid OrderId)
        {
            if (OrderId == null || OrderId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.OrderHeader.FirstOrDefaultAsync(o => o.Id.ToString().Equals(OrderId.ToString()));
            if (result == null) return NotFound();
            
            result.Status = SD.Status.orderCanceled;
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder","Order");
        }

        [Authorize(Roles = (SD.KitchenUser + "," + SD.ManagerUser))]
        public async Task<IActionResult> OrderReady(Guid OrderId)
        {
            if (OrderId == null || OrderId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.OrderHeader.FirstOrDefaultAsync(o => o.Id.ToString().Equals(OrderId.ToString()));
            if (result == null) return NotFound();

            result.Status = SD.Status.orderReadyForPickup;
            await _db.SaveChangesAsync();

            //Email logic

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail=null, string searchName = null, string searchPhone = null)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //if (claim == null) return NotFound();

            OrderListViewModel orderListViewModel = new OrderListViewModel
            {
                Orders = new List<OrderDetailsViewModel>(),
            };

            //stack of querries for linq statement
            //Stack<Func<OrderHeader, bool>> linqConditionsStack = new Stack<Func<OrderHeader, bool>>();



            var predicateBuilder = PredicateBuilder.New<OrderHeader>(true);
            predicateBuilder.Start(o => o.Status.Equals(SD.Status.orderReadyForPickup));

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("/Customer/Order/OrderPickup?productPage=:");
            stringBuilder.Append("&searchEmail=");
            if (searchEmail!=null)
            {
                stringBuilder.Append(searchEmail);
                predicateBuilder.And(o => o.User.Email.ToLower().Equals(searchEmail.ToLower()));

                //add condition to check
                //linqConditionsStack.Push(new Func<OrderHeader, bool>(o => o.User.Email.ToLower().Equals(searchEmail)));
            }
            stringBuilder.Append("&searchName=");
            if (searchName != null)
            {
                stringBuilder.Append(searchName);
                predicateBuilder.And(o => o.PickupName.ToLower().Equals(searchName.ToLower()));
                //linqConditionsStack.Push(new Func<OrderHeader, bool>(o => o.PickupName.ToLower().Equals(searchName)));
            }
            stringBuilder.Append("&searchPhone=");
            if (searchPhone != null)
            {
                stringBuilder.Append(searchPhone);
                predicateBuilder.And(o => o.PhoneNumber.Equals(searchPhone));
                //linqConditionsStack.Push(new Func<OrderHeader, bool>(o => o.User.Email.ToLower().Equals(searchPhone)));
            }

            //var predicateCompiled = predicateBuilder.Compile();

            //List<OrderHeader> orderHeaderList = null;
            var orderHeaderList = await _db.OrderHeader.Include(o => o.User).Where(predicateBuilder).ToListAsync();

            //while (linqConditionsStack.Count>0)
            //{
            //    orderHeaderList = orderHeaderList.Where(linqConditionsStack.Pop()).ToList();
            //}


            //var orderHeaderList = await _db.OrderHeader.Include(o => o.User).Where(o => o.Status.Equals(SD.Status.orderReadyForPickup)).ToListAsync();

            foreach (var item in orderHeaderList)
            {
                orderListViewModel.Orders.Add(new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId.ToString().Equals(item.Id.ToString())).ToListAsync()
                });
            }

            var count = orderListViewModel.Orders.Count;
            orderListViewModel.Orders = orderListViewModel.Orders.OrderByDescending(p => p.OrderHeader.Id)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();
            orderListViewModel.PagingInfo = new Models.PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = stringBuilder.ToString() //we'll replace ':' it the view, with the page number
            };

            return View(orderListViewModel);
        }


        [Authorize(Roles = (SD.ManagerUser + "," + SD.FrontDeskUser))]
        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(Guid orderId) 
        {
            if (orderId == null || orderId.Equals(Guid.Empty)) return NotFound();
            var result = await _db.OrderHeader.FirstOrDefaultAsync(o => o.Id.ToString().Equals(orderId.ToString()));
            if (result == null) return NotFound();

            result.Status = SD.Status.orderCompleted;
            await _db.SaveChangesAsync();

            return RedirectToAction("OrderPickup", "Order");
        }
    }
}
