using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Restaurant.Models;

namespace Restaurant.Filters
{
    public class LoginStatusFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //檢查使用者是否已登入(Session是否有值)
            if (context.HttpContext.Session.GetString("UserInfo") == null)
            {
                //    //尚未登入，導向登入頁面(Action,Controller,null)
                context.Result = new RedirectToActionResult("Login", "UserLogin", null);

                //    //到Program.cs中註冊此Filter
                //    //builder.Services.AddScoped<LoginStatusFilter>();

                //    //到TablesController.cs(或必須登入的Controllerc或者Action)加上此Filter作為驗證器
                //    //[ServiceFilter(typeof(LoginStatusFilter))]

            }


        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        
    }
}
