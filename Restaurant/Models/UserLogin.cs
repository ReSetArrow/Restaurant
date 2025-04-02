using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models;

public partial class UserLogin
{
    public int UserID { get; set; }

    
    public string Account { get; set; } = null!;

   
    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ID { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string UserNumber { get; set; } = null!;

    public string? ManagerNumber { get; set; }

    public string? RoleCode { get; set; }

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual Role? RoleCodeNavigation { get; set; }
}
