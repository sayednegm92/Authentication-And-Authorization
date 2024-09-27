using AuthenticationApp.DTO;
using AuthenticationApp.Model;


namespace AuthenticationApp.Services
{
    public interface IProductService
    {
        Task<ProductReturn> AddProduct(Product product);
        Task<ProductReturn> EditProduct(Product product);
        Task<ProductReturn> RemoveProduct(int id);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
    }
}
