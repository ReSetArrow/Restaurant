using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    public class RolesController : Controller
    {
        private readonly RestaurantContext _context;

        public RolesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Role.ToListAsync());
        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.RoleCode == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleCode,Title,Supervisor")] Role role)
        {
            if (ModelState.IsValid)
            {
                // 檢查主鍵是否重複
                if (_context.Role.Any(r => r.RoleCode == role.RoleCode))
                {
                    ModelState.AddModelError("RoleCode", "角色代碼重複");
                    return View(role);
                }
                if(_context.Role.Any(r => r.Title == role.Title))
                {
                    ModelState.AddModelError("Title", "職稱重複");
                    return View(role);
                }
                _context.Add(role);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"新增成功：{role.RoleCode} - {role.Title}";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RoleCode,Title,Supervisor")] Role role)
        {
            if (id != role.RoleCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // 檢查 Title 是否重複
                if (await _context.Role.AnyAsync(p => p.Title == role.Title && p.RoleCode != role.RoleCode ))
                {
                    ModelState.AddModelError("Title", "職稱已存在。");
                    return View(role);
                }
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"修改成功：{role.RoleCode} - {role.Title}";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleCode))
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
            return View(role);
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Role
                .FirstOrDefaultAsync(m => m.RoleCode == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role != null)
            {
                //////////////////////GPT////////////////////////////
                // 檢查是否有依賴資料
                bool hasOrderDependencies = await _context.Order.AnyAsync(o => o.StatusCode == role.RoleCode);
                bool hasUserLoginDependencies = await _context.UserLogin.AnyAsync(u => u.RoleCode == role.RoleCode);
                if (hasOrderDependencies || hasUserLoginDependencies)
                {
                    // 有依賴資料，給予提示並阻止刪除操作
                    TempData["SuccessMessage"] = $"◢▆▅▄▃崩╰(〒皿〒)╯潰▃▄▅▇◣無法刪除：{role.RoleCode} - {role.Title}，因為有依賴的訂單或使用者資料。";
                    return RedirectToAction(nameof(Index));
                }
                //////////////////////GPT////////////////////////////
                _context.Role.Remove(role);
                //加上TempData記錄刪除，可以在Index回傳刪除成功訊息
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"刪除成功：{role.RoleCode} - {role.Title}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(string id)
        {
            return _context.Role.Any(e => e.RoleCode == id);
        }
    }
}
