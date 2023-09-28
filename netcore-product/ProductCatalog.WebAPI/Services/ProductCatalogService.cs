using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.WebAPI.Data;
using ProductCatalog.WebAPI.Models;

namespace ProductCatalog.WebAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductContext _productContext;

        public ProductService(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _productContext.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetProductsByName(string name)
        {
            return await _productContext.Products
                .Where(b => b.Name.Contains(name))
                .ToListAsync();
        }


        public async Task AddProduct(Product product)
        {
            _productContext.Products.Add(product);
            await _productContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _productContext.Products.Update(product);
            await _productContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _productContext.Products.FindAsync(id);
            if (product != null)
            {
                _productContext.Products.Remove(product);
                await _productContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAllProducts()
        {
            _productContext.Products.RemoveRange(_productContext.Products);
            await _productContext.SaveChangesAsync();
        }


        public async Task<List<Product>> SortProductsByName(string sortOrder)
        {
            var products = await _productContext.Products.ToListAsync();
            return sortOrder.ToLower() == "desc" ? products.OrderByDescending(b => b.Name).ToList() : products.OrderBy(b => b.Name).ToList();
        }

        public async Task<List<Product>> SortProductsByCategory(string sortOrder)
        {
            var products = await _productContext.Products.ToListAsync();
            return sortOrder.ToLower() == "desc" ? products.OrderByDescending(b => b.Category).ToList() : products.OrderBy(b => b.Category).ToList();
        }


        public async Task<List<Product>> SortProductsByPrice(string sortOrder)
        {
            var products = await _productContext.Products.ToListAsync();
            return sortOrder.ToLower() == "desc" ? products.OrderByDescending(b => b.Price).ToList() : products.OrderBy(b => b.Price).ToList();
        }


         public async Task<int> GetTotalProductCount()
        {
            return await _productContext.Products.CountAsync();
        }

        public async Task<List<Product>> GetProductsByCategory(string category)
        {
    
            return await _productContext.Products
            .Where(p => p.Category == category)
            .ToListAsync();
        }

        
    }
}
