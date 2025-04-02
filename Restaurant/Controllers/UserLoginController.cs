using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Restaurant.Models;
using Newtonsoft.Json;
using NuGet.Protocol;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Restaurant.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly RestaurantContext _context;
        public UserLoginController(RestaurantContext context)
        {
            _context = context;
        }
   

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            //將登入時的密碼雜湊後再放進資料庫比對，若用明碼會失敗
            var result = await _context.UserLogin.Where(m => m.Account == userLogin.Account && m.Password == _context.ComputeSha256Hash(userLogin.Password)).FirstOrDefaultAsync();

            //如果帳密正確,導入後台頁面
            if (result != null)
            {
                //發給證明,證明他已經登入，須將資料轉成json格式
                HttpContext.Session.SetString("UserInfo", result.ToJson());

                return RedirectToAction("Index", "TableList");
            }
            else //如果帳密不正確,回到登入頁面,並告知帳密錯誤
            {
                ViewData["Message"] = "帳號或密碼錯誤";
            }

            return View(result);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ...

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(UserLogin userLogin)
        {
            // UserID使用資料庫流水號
            // 移除UserNumber的驗證,交由資料庫組合產生
            ModelState.Remove("UserNumber");

            //呼叫RestaurantContextHash的雜湊方法
            userLogin.Password = _context.ComputeSha256Hash(userLogin.Password);

            var account = _context.UserLogin.Where(m => m.Account == userLogin.Account).FirstOrDefault();
            // 檢查帳號是否重複
            if (account != null)
            {
                ViewData["Message"] = "帳號重複";
                return View(userLogin);
            }

            if (ModelState.IsValid)
            {
                // 生成新的 UserNumber(GPT)
                //var roleCode = userLogin.RoleCode ?? "MB"; // 假設 RoleCode 預設為 "MB"
                //userLogin.UserNumber = (await _context.UserLogin.FromSqlRaw(
                //    "SELECT dbo.fnGetNewUserNumber({0})", roleCode).ToListAsync()).FirstOrDefault()?.UserNumber;

                //_context.Add(userLogin);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Login));


                // 生成新的 UserNumber(GPT)
                var roleCode = userLogin.RoleCode ?? "MB"; // 假設 RoleCode 預設為 "MB"
                var newUserNumber = string.Empty;

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT dbo.fnGetNewUserNumber(@RoleCode)";
                    command.Parameters.Add(new SqlParameter("@RoleCode", roleCode));

                    _context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (result.Read())
                        {
                            newUserNumber = result.GetString(0);
                        }
                    }
                }

                userLogin.UserNumber = newUserNumber;

                _context.Add(userLogin);
                await _context.SaveChangesAsync();

                //新增TempData，用來顯示註冊成功訊息
                TempData["RegisterMessage"] = "OK";
                return RedirectToAction(nameof(Login));
            }
            return View(userLogin);
        }
        //設計一個布林值函數，用來與資料庫檢查帳號是否重複
        public bool CheckAccount(string account)
        {
            var result = _context.UserLogin.Where(m => m.Account == account).FirstOrDefault();

            //停留3秒作為驗證等待時間
            Task.Delay(3000).Wait();

            return result == null;
        }

        //實作Logout功能
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserInfo");
            TempData["LogoutMessage"]="OK";
            return RedirectToAction("Login", "UserLogin");
        }
    }
}
