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
    public class StatusController : Controller
    {
        private readonly RestaurantContext _context;

        public StatusController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Status
        public async Task<IActionResult> Index()
        {
            return View(await _context.Status.ToListAsync());
        }

        // GET: Status/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Status
                .FirstOrDefaultAsync(m => m.StatusCode == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // GET: Status/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Status/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusCode,StatusCategory")] Status status)
        {
            if (ModelState.IsValid)
            {
                // 檢查主鍵是否重複
                if (await _context.Status.AnyAsync(p => p.StatusCode == status.StatusCode))
                {
                    ModelState.AddModelError("StatusCode", "狀態代碼已存在。");
                    return View(status);
                }
                if (await _context.Status.AnyAsync(p => p.StatusCategory == status.StatusCategory))
                {
                    ModelState.AddModelError("StatusCategory", "訂單狀態已存在。");
                    return View(status);
                }
                _context.Add(status);
                await _context.SaveChangesAsync(); 
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"新增成功：{status.StatusCode} - {status.StatusCategory}";
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }

        // GET: Status/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Status.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }

        // POST: Status/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StatusCode,StatusCategory")] Status status)
        {
            if (id != status.StatusCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {// 檢查 PaymentType 是否重複
                if (await _context.Status.AnyAsync(p => p.StatusCategory == status.StatusCategory && p.StatusCode != status.StatusCode))
                {
                    ModelState.AddModelError("StatusCategory", "訂單狀態已存在。");
                    return View(status);
                }
                try
                {
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] =$"編輯成功：{status.StatusCode} - {status.StatusCategory}";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusExists(status.StatusCode))
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
            return View(status);
        }

        // GET: Status/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Status
                .FirstOrDefaultAsync(m => m.StatusCode == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var status = await _context.Status.FindAsync(id);
            if (status != null)
            {
                //////////////////////GPT////////////////////////////
                // 檢查是否有依賴資料
                bool hasOrderDependencies = await _context.Order.AnyAsync(o => o.StatusCode == status.StatusCode);
                if (hasOrderDependencies)
                {
                    // 有依賴資料，給予提示並阻止刪除操作
                    TempData["SuccessMessage"] = $"◢▆▅▄▃崩╰(〒皿〒)╯潰▃▄▅▇◣無法刪除：{status.StatusCode} - {status.StatusCategory}，因為有依賴的訂單資料。";
                    return RedirectToAction(nameof(Index));
                }
                //////////////////////GPT////////////////////////////
                _context.Status.Remove(status);
                //加上TempData記錄刪除，可以在Index回傳刪除成功訊息
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"刪除成功：{status.StatusCode} - {status.StatusCategory}";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatusExists(string id)
        {
            return _context.Status.Any(e => e.StatusCode == id);
        }
    }
}
