using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models;

public partial class Status
{
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "必須是兩個小寫字母")]
    [Display(Name = "狀態代碼")]
    public string StatusCode { get; set; } = null!;

    [Required(ErrorMessage = "狀態類別是必填項目")]
    [StringLength(20, ErrorMessage = "狀態類別不能超過20個字元")]
    [Display(Name = "狀態")]
    public string StatusCategory { get; set; } = null!;

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();
}