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
    public class LoginUserController : Controller
    {
        private readonly RestaurantContext _context;

        public LoginUserController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: LoginUser
        public async Task<IActionResult> Index()
        {
            var restaurantContext = _context.UserLogin.Include(u => u.RoleCodeNavigation);
            return View(await restaurantContext.ToListAsync());
        }

        // GET: LoginUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogin
                .Include(u => u.RoleCodeNavigation)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }


        [HttpGet]
        public async Task<IActionResult> CheckManager(string managerNumber)
        {

            //var result = _context.UserLogin.Where(m => m.UserNumber == managerNumber).FirstOrDefault();
            //return result == null;
            // 如果 managerNumber 為空或 null，直接返回 true
            if (string.IsNullOrEmpty(managerNumber))
            {
                return Json(true);
            }
            //GPT推薦非同步方法
            var managerExists = await _context.UserLogin
                .Include(u => u.RoleCodeNavigation)//加上Role的Supervisor必須為true
                .AnyAsync(u => u.UserNumber == managerNumber && u.RoleCodeNavigation.Supervisor);
            //停留2秒作為驗證等待時間
            Task.Delay(2000).Wait();
            return Json(managerExists); // 返回 true 表示 ManagerNumber 存在，false 表示 ManagerNumber 不存在
           
        }

        // GET: LoginUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogin.FindAsync(id);
            if (userLogin == null)
            {
                return NotFound();
            }
            ViewData["RoleCode"] = new SelectList(_context.Role, "RoleCode", "Title", userLogin.RoleCode);
            return View(userLogin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,Name,Birthday,PhoneNumber,ManagerNumber,RoleCode")] UserLogin userLogin)
        {
            // 進入判斷前須忽略
            ModelState.Remove("UserID");
            ModelState.Remove("UserNumber");
            ModelState.Remove("Account");
            ModelState.Remove("Password");
            ModelState.Remove("ID");
            ModelState.Remove("PasswordConfirm");

            if (ModelState.IsValid)
            {
                try
                {
                    // 從資料庫中獲取現有的 UserLogin 資料
                    var existingUserLogin = await _context.UserLogin.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == id);
                    if (existingUserLogin == null)
                    {
                        return NotFound();
                    }

                    // 設置不可修改的欄位
                    userLogin.Account = existingUserLogin.Account;
                    userLogin.Password = existingUserLogin.Password;
                    userLogin.PasswordConfirm = existingUserLogin.Password;
                    userLogin.UserNumber = existingUserLogin.UserNumber;
                    userLogin.ID = existingUserLogin.ID;


                    _context.Update(userLogin);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"修改成功";
                    return RedirectToAction("Details", "LoginUser", new { id = userLogin.UserID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserLoginExists(userLogin.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["RoleCode"] = new SelectList(_context.Role, "RoleCode", "Title", userLogin.RoleCode);
            return View(userLogin);
        }

        private bool UserLoginExists(int id)
        {
            return _context.UserLogin.Any(e => e.UserID == id);
        }
    }
}
