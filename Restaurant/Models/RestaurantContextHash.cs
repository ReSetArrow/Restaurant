using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Restaurant.Models;

public partial class RestaurantContext : DbContext
{

    //加入哈希碼，與UserLogin互動
    public string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    //呼叫加入新訂單非同步方法(預存程序)
    public async Task<int> ExecSPAddNewOrderAsync(string StatusCode, string UserNumber, string PaymentCode, string ExpectedArrivalDate, string Cart)
    {
        //呼叫預存程序
        var results= this.Database.ExecuteSqlRawAsync("EXEC AddNewOrder {0},{1},{2},{3},{4}", StatusCode, UserNumber, PaymentCode, ExpectedArrivalDate, Cart);
        return await results;

        //GPT
        //SqlParameter 類別來建立參數，並將這些參數傳遞給 ExecuteSqlRawAsync 方法。
        //使用 SqlParameter 可以防止 SQL 注入攻擊，因為參數會被自動轉義
   //     var parameters = new[]
   //{
   //     new SqlParameter("@StatusCode", StatusCode),
   //     new SqlParameter("@UserNumber", UserNumber),
   //     new SqlParameter("@PaymentCode", PaymentCode),
   //     new SqlParameter("@ExpectedArrivalDate", ExpectedArrivalDate),
   //     new SqlParameter("@Cart", Cart)
   // };
        //return await Database.ExecuteSqlRawAsync("EXEC dbo.AddNewOrder @StatusCode, @UserNumber, @PaymentCode,  @ExpectedArrivalDate, @Cart", parameters);

    }
    
}