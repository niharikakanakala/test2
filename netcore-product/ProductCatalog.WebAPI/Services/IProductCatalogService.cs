using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalog.WebAPI.Models;

namespace ProductCatalog.WebAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<List<Product>> GetProductsByName(string name); 
        Task<int> GetTotalProductCount();
        Task<List<Product>> GetProductsByCategory(string category);
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(int id);
        Task DeleteAllProducts();
        Task<List<Product>> SortProductsByName(string sortOrder);
        Task<List<Product>> SortProductsByCategory(string sortOrder);
        Task<List<Product>> SortProductsByPrice(string sortOrder);

    }
}