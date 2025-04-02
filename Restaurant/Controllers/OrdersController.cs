using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Restaurant.Filters;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    [ServiceFilter(typeof(LoginStatusFilter))]
    public class OrdersController : Controller
    {
        private readonly RestaurantContext _context;

        public OrdersController(RestaurantContext context)
        {
            _context = context;
        }
        private UserLogin getUserInfo()
        {
            var userLogin = HttpContext.Items["UserLogin"] as UserLogin;
            return userLogin;
        }
     
        public async Task<IActionResult> Index()
        {
            var userLogin = getUserInfo();
            //GPT:IQueryable<Order> 是一個延遲執行的查詢，允許您在查詢執行之前構建查詢。
            IQueryable<Order> restaurantContext;

            if (userLogin.RoleCode != "MB")
            {
                // 如果使用者的角色代碼不是 "MB"，顯示所有訂單
                restaurantContext = _context.Order
                    .Include(o => o.PaymentCodeNavigation)
                    .Include(o => o.StatusCodeNavigation)
                    .Include(o => o.UserNumberNavigation)
                    .Include(o => o.OrderDetails) // 確認是否有關聯到 OrderDetails
                    .ThenInclude(od => od.Table); // 確認是否有關聯到 Table
            }
            else
            {
                // 如果使用者的角色代碼是 "MB"，只顯示自己的訂單
                restaurantContext = _context.Order
                    .Include(o => o.PaymentCodeNavigation)
                    .Include(o => o.StatusCodeNavigation)
                    .Include(o => o.UserNumberNavigation)
                    .Include(o => o.OrderDetails) // 確認是否有關聯到 OrderDetails
                    .ThenInclude(od => od.Table) // 確認是否有關聯到 Table
                    .Where(o => o.UserNumber == userLogin.UserNumber); // 使用者只能看到自己的訂單
            }

            return View(await restaurantContext.ToListAsync());
            //第10集最後Total實作
            //var Orders = _context.Order.Include(o => o.UserNumber)
            //    .Where(o => o.UserNumber == getUserInfo().UserNumber)
            //    .Select(o => new
            //    {
            //        OrderNumber = o.OrderNumber,
            //        OrderDate = o.OrderDate,
            //        StatusCode = o.StatusCode,
            //        UserNumber = o.UserNumber,
            //        PaymentCode = o.PaymentCode,
            //        ExpectedArrivalDate = o.ExpectedArrivalDate,
            //        Total = o.OrderDetails.Sum(od => od.Pricing*od.Qty)

            //    });


        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.PaymentCodeNavigation)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.UserNumberNavigation)
                .FirstOrDefaultAsync(m => m.OrderNumber == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentType");
            return View();
            //ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentCode");
            //ViewData["StatusCode"] = new SelectList(_context.Status, "StatusCode", "StatusCode");
            //ViewData["TableID"] = new SelectList(_context.Table, "TableID", "TableID");
            //ViewData["UserNumber"] = new SelectList(_context.UserLogin, "UserNumber", "UserNumber");

        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderNumber,OrderDate,StatusCode,UserNumber,PaymentCode,ExpectedArrivalDate")] Order order, string Cart, int Year, int Month, int Day, int Hour, int Minute)
        {
            // 設定訂單日期為現在時間
            order.OrderDate = DateTime.Now;
            // 設定下訂的使用者為目前登入的使用者
            order.UserNumber = (HttpContext.Items["UserLogin"] as UserLogin).UserNumber;

            // 使用 SetExpectedArrivalDate 方法設置 ExpectedArrivalDate
            string expectedArrivalDateToString = $"{Year}{Month:D2}{Day:D2}{Hour:D2}{Minute:D2}";
            order.ExpectedArrivalDate = expectedArrivalDateToString;

            //驗證時忽略 OrderNumber 和 UserNumber
            ModelState.Remove("OrderNumber");
            ModelState.Remove("UserNumber");

            //忽略關聯屬性
            ModelState.Remove("StatusCodeNavigation");
            ModelState.Remove("UserNumberNavigation");
            ModelState.Remove("PaymentCodeNavigation");

            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors);
            //    foreach (var error in errors)
            //    {
            //        Console.WriteLine(error.ErrorMessage);
            //    }
            //    return View(order);
            //}

            if (ModelState.IsValid)
            {
                //存入資料庫的動作相對複雜，所以使用SQL語法
                //使用資料庫端的預存程序
                //_context.Add(order);，將預存程序建立好後就不需要此行

                //避免例外使用try處理
                try
                {
                    ///////////////////////////GPT///////////////////////////
                    // 反序列化 Cart 字串為 OrderDetails 物件列表
                    //var orderDetailsList = JsonConvert.DeserializeObject<List<OrderDetails>>(Cart);

                    //// 確認每個 OrderDetails 物件的 Pricing 屬性不為 null
                    //foreach (var item in orderDetailsList)
                    //{
                    //    if (item.Pricing == 0)
                    //    {
                    //        ModelState.AddModelError("Pricing", "訂價不能為 0");
                    //        return View(order);
                    //    }
                    //}
                    ///////////////////////////GPT///////////////////////////
                    var result = await _context.ExecSPAddNewOrderAsync(order.StatusCode, order.UserNumber, order.PaymentCode, order.ExpectedArrivalDate, Cart);
                    //await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        TempData["Message"] = "OK";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch
                {
                    return View(order);
                }
            }
            ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentCode", order.PaymentCode);
            ViewData["StatusCode"] = new SelectList(_context.Status, "StatusCode", "StatusCode", order.StatusCode);
            //ViewData["UserNumber"] = new SelectList(_context.UserLogin, "UserNumber", "UserNumber", order.UserNumber);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentCode", order.PaymentCode);
            ViewData["StatusCode"] = new SelectList(_context.Status, "StatusCode", "StatusCode", order.StatusCode);
            ViewData["UserNumber"] = new SelectList(_context.UserLogin, "UserNumber", "UserNumber", order.UserNumber);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("OrderNumber,OrderDate,StatusCode,UserNumber,PaymentCode")] Order order)
        {
            if (id != order.OrderNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentCode", order.PaymentCode);
            ViewData["StatusCode"] = new SelectList(_context.Status, "StatusCode", "StatusCode", order.StatusCode);
            ViewData["UserNumber"] = new SelectList(_context.UserLogin, "UserNumber", "UserNumber", order.UserNumber);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.PaymentCodeNavigation)
                .Include(o => o.StatusCodeNavigation)
                .Include(o => o.UserNumberNavigation)
                .FirstOrDefaultAsync(m => m.OrderNumber == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(string id)
        {
            return _context.Order.Any(e => e.OrderNumber == id);
        }

      

    }
}
