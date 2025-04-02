using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly RestaurantContext _context;

        public PaymentsController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Payment.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.PaymentCode == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            var user = HttpContext.Items["UserLogin"] as UserLogin;
            ViewData["UserAccount"] = user?.Account;
            ViewData["PaymentCode"] = new SelectList(_context.Payment, "PaymentCode", "PaymentType");

            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PaymentCode,PaymentType")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(payment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(payment);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentCode,PaymentType")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // 檢查主鍵是否重複
                if (await _context.Payment.AnyAsync(p => p.PaymentCode == payment.PaymentCode))
                {
                    ModelState.AddModelError("PaymentCode", "付款代碼已存在。");
                    return View(payment);
                }
                if(await _context.Payment.AnyAsync(p => p.PaymentType == payment.PaymentType))
                {
                    ModelState.AddModelError("PaymentType", "付款方式已存在。");
                    return View(payment);
                }

                _context.Add(payment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] =$"新增成功：{payment.PaymentCode} - {payment.PaymentType}";
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PaymentCode,PaymentType")] Payment payment)
        {
            if (id != payment.PaymentCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 檢查 PaymentType 是否重複
                if (await _context.Payment.AnyAsync(p => p.PaymentType == payment.PaymentType && p.PaymentCode != payment.PaymentCode))
                {
                    ModelState.AddModelError("PaymentType", "付款方式已存在。");
                    return View(payment);
                }
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"修改成功{payment.PaymentCode} - {payment.PaymentType}";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentCode))
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
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.PaymentCode == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment != null)
            {//////////////////////GPT////////////////////////////
             // 檢查是否有依賴資料
                bool hasOrderDependencies = await _context.Order.AnyAsync(o => o.PaymentCode == payment.PaymentCode);
                if (hasOrderDependencies)
                {
                    // 有依賴資料，給予提示並阻止刪除操作
                    TempData["SuccessMessage"] = $"◢▆▅▄▃崩╰(〒皿〒)╯潰▃▄▅▇◣無法刪除：{payment.PaymentCode} - {payment.PaymentType}，因為有依賴的訂單資料。";
                    return RedirectToAction(nameof(Index));
                }
                //////////////////////GPT////////////////////////////
                _context.Payment.Remove(payment);
                //加上TempData記錄刪除，可以在Index回傳刪除成功訊息
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"刪除成功：{payment.PaymentCode} - {payment.PaymentType}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(string id)
        {
            return _context.Payment.Any(e => e.PaymentCode == id);
        }
    }
}
