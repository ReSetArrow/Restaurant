using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Restaurant.Models;

namespace Restaurant.Filters
{
    public class LogFilter:IActionFilter
    {
        //Action執行之前
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var UserJson = Context.Session.GetString("UserInfo");
            var UserJson = context.HttpContext.Session.GetString("UserInfo");
            if (UserJson != null)
            {
                var UserInfo=  JsonConvert.DeserializeObject<UserLogin>(UserJson);
                context.HttpContext.Items["UserLogin"] = UserInfo;
            }
           
        }
        //Action執行之後
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("LogFilter: OnActionExecuted called");

            //抓資訊
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var agent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            string user= "Guest";
            var time=DateTime.Now;


            //判斷登入是否為空，並抓取使用者資訊
            var UserInfoJson = context.HttpContext.Session.GetString("UserInfo");
            if (UserInfoJson != null)
            {
                var LoginUser = JsonConvert.DeserializeObject<UserLogin>(UserInfoJson);
                user = LoginUser?.UserNumber + " - " + LoginUser?.Account+" - "+LoginUser?.Name;
            }

            //組合並寫入Log，中間用小逗號隔開方便Excel自動分格
            string log = $"{time},使用者:{user},IP{ip},瀏覽器:{agent},Controller:{controller},Action:{action}";
            //新增資料夾LogFiles並寫檔(寫成ActionLog檔案副檔名為csv，可以用Excel開啟，也可用記事本開啟)
            string path = "LogFiles/ActionLog.csv";
            using(StreamWriter sw = new StreamWriter(path,true, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(log);
            }

            //此為全域的Filter，所以要到Program.cs找到下方程式碼註冊
            //builder.Services.AddControllersWithViews();
            //實作後所有在後端相關的行為皆會被記錄
        }

    }
}
