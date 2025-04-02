using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.ViewComponents
{
    public class VCGetOrderDetailByOrderID : ViewComponent
    {
        private readonly RestaurantContext _context;
        public VCGetOrderDetailByOrderID(RestaurantContext context)
        {
            _context = context;
        }

        //Details
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            //var orderDetails = await _context.OrderDetails.Where(o => o.OrderNumber == id).ToListAsync();
            var orderDetails = await _context.OrderDetails
                .Include(od => od.OrderNumberNavigation) // 載入 Order 的相關資料
                .Include(od => od.Table) // 載入 Table 的相關資料
                .Where(o => o.OrderNumber == id)
                .ToListAsync();


            if (orderDetails == null)
            {
                return View("NotFound");
            }

            return View(orderDetails);
        }
    }
}
