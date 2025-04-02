using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Restaurant.Models;

public partial class Payment
{
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "兩個大寫英文字符。")]
    [Display(Name = "付款代碼")]
    public string PaymentCode { get; set; } = null!;

    [Required(ErrorMessage = "必填")]
    [StringLength(20, ErrorMessage = " 20 個字以內")]
    [Display(Name = "付款方式")]
    public string PaymentType { get; set; } = null!;

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();
}