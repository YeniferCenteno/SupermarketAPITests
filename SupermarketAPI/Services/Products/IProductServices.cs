using SupermarketAPI.DTOs;

namespace SupermarketAPI.Services.Products
{
    public interface IProductServices
    {
        Task<int> PostProduct(ProductRequest product);
        Task<List<ProductResponse>> GetProducts();
        Task<ProductResponse> GetProduct(int productId);
        Task<int> PutProduct(int productId, ProductRequest product);
        Task<int> DeleteProduct(int productId);

    }
}
