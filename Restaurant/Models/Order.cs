using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models;

public partial class Order
{
    public string OrderNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public  string ExpectedArrivalDate { get; set; }

    public string StatusCode { get; set; } = null!;

    public string UserNumber { get; set; } = null!;

    public string PaymentCode { get; set; } = null!;

    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    public virtual Payment PaymentCodeNavigation { get; set; } = null!;

    public virtual Status StatusCodeNavigation { get; set; } = null!;

    public virtual UserLogin UserNumberNavigation { get; set; } = null!;
   
}
