using System;
using System.Collections.Generic;

namespace SupermarketAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
