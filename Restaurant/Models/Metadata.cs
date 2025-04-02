using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models;

public partial class UserData
{
    [Display(Name = "帳號",Prompt = "帳號3-50碼")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "帳號為3-50碼")]
    [RegularExpression("[A-Za-z][A-Za-z0-9]{2,49}", ErrorMessage = "帳號格式有誤")]
    [Required(ErrorMessage = "必填")]
    public string Account { get; set; } = null!;

    [Display(Name = "密碼", Prompt = "密碼6-50碼")]
    [Required(ErrorMessage = "必填")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "密碼為6-50碼")]
    [MinLength(6)]
    [MaxLength(50)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "姓名")]
    [Required(ErrorMessage = "必填")]
    public string Name { get; set; } = null!;

    [Display(Name = "身分證")]
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[A-Z][1-2][0-9]{8}$", ErrorMessage = "格式錯誤")]
    public string ID { get; set; } = null!;

    [Display(Name = "生日")]
    [Required(ErrorMessage = "必填")]
    [DataType(DataType.Date)]
    public DateTime Birthday { get; set; }

    [Display(Name = "電話")]
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[0][9][0-9]{8}$", ErrorMessage = "格式錯誤")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "主管員編")]
    
    public string? ManagerNumber { get; set; }

}

//新增一個確認密碼有無錯誤的方式，實作於View/UserLogins的Create
//利用驗證器，使其繼承原本Models內容但在回寫資料庫時並不回寫此欄位
[ModelMetadataType(typeof(UserData))]
public partial class UserLogin
{
    //告訴EF不要回寫此欄位給資料庫
    [NotMapped]
    [Display(Name = "再次確認密碼", Prompt = "")]
    [Required(ErrorMessage = "必填")]
    [Compare(nameof(Password), ErrorMessage = "密碼兩次輸入不相同")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; } = null!;
}


public partial class OrderDetails
{
    //public decimal SubTotal=> Pricing * Qty;
    //不回傳給資料庫  並寫成無小數的整數
    [NotMapped]
    [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
    public decimal SubTotal
    {
        get
        {
            return Pricing * Qty;
        }
    }
}


public partial class OrderData
{
    [Required(ErrorMessage = "必填")]
    [StringLength(12, ErrorMessage = "不能超過9個字元")]
    [Display(Name = "訂單號碼")]
    public string OrderNumber { get; set; } = null!;

    [Required(ErrorMessage = "必填")]
    [Display(Name = "訂單日期")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "必填")]
    [Display(Name = "預計用餐日期")]
    public string ExpectedArrivalDate { get; set; }

    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "狀態代碼必須是兩個小寫字母")]
    [Display(Name = "狀態代碼")]
    public string StatusCode { get; set; } = null!;

    public string UserNumber { get; set; } = null!;

    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "付款代碼必須是兩個大寫字母")]
    [Display(Name = "付款代碼")]
    public string PaymentCode { get; set; } = null!;

}

[ModelMetadataType(typeof(OrderData))]
public partial class Order
{
    [NotMapped]
    [Display(Name = "訂單總金額")]
    [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
    public decimal Total
    {
        get
        {
            decimal total = 0;
            foreach (var item in OrderDetails)
            {
                total += item.SubTotal;
            }
            return total;
        }
    }
}