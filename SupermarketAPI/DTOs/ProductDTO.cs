using SupermarketAPI.Models;

namespace SupermarketAPI.DTOs
{
    public class ProductResponse
    {
        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }

        public bool ProductStatus { get; set; }

        //public virtual UserResponse User { get; set; } = null!;
    }
    
    public class ProductRequest
    {
        //public int ProductId { get; set; }

        public int UserId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }

        public bool ProductStatus { get; set; }
    }
}
