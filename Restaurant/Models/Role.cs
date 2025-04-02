using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Models;

public partial class Role
{
    [Required(ErrorMessage = "必填")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "狀態代碼必須是兩個大寫字母")]
    [Display(Name = "角色代碼")]
    public string RoleCode { get; set; } = null!;

    [Required(ErrorMessage = "必填")]
    [StringLength(50, ErrorMessage = "不能超過50個字元")]
    [Display(Name = "職稱")]
    public string Title { get; set; } = null!;

    [Display(Name = "主管職")]
    public bool Supervisor { get; set; }

    public virtual ICollection<UserLogin> UserLogin { get; set; } = new List<UserLogin>();
}
