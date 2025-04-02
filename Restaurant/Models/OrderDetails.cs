using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models;

public partial class OrderDetails
{
    public string OrderNumber { get; set; } = null!;

    public string TableID { get; set; } = null!;

    [Required(ErrorMessage = "必填")]
    [Display(Name = "訂價")]
    [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
    public decimal Pricing { get; set; }

    public int Qty { get; set; } 

    public virtual Order OrderNumberNavigation { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
