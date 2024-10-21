using System;
using System.Collections.Generic;

namespace SupermarketAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal ProductPrice { get; set; }

    public bool ProductStatus { get; set; }

    public virtual User User { get; set; } = null!;
}
