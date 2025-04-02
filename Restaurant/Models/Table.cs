using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models;

public partial class Table
{
    [Key]
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[A-Z][1-3][1-9]$", ErrorMessage = "[A-Z]區[1-3]樓[1-9]房")]
    [Display(Name ="廳位")]
    public string TableID { get; set; } = null!;


    [Required(ErrorMessage = "必填")]
    [Range(1, 20, ErrorMessage = "人數必須在1到20之間")]
    [Display(Name = "人數")]
    public int Limit { get; set; }


    [Required(ErrorMessage = "必填")]
    [Display(Name = "訂價")]
    [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
    public decimal Pricing { get; set; }

    [Display(Name = "備註")]
    public string Remark { get; set; } = null!;


    [Display(Name = "圖片")]
    public string? Picture { get; set; }

    [HiddenInput]
    public string? ImageType { get; set; }

    //public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}
