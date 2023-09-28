using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text;
using ProductCatalog.WebAPI.Data;
using ProductCatalog.WebAPI.Models;
using ProductCatalog.WebAPI;
using ProductCatalog.WebAPI.Services;

namespace SampleIntegrationTests.Tests
{
    public class ProductCatalogIntegrationTests
    {
        private TestServer _server;
        private HttpClient _client;

        public ProductCatalogIntegrationTests()
        {
            SetUpClient();
        }

       private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    string connectionString = "Server='localhost';Database='ProductDatabase';User='sa';Password='Test-hackerearth';";
                    services.AddDbContext<ProductContext>(options => options.UseSqlServer(connectionString));
                    services.AddTransient<IProductService, ProductService>();
                    services.AddControllers();
                });

            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }

        private void ClearTasksTable()
        {
            using (var scope = _server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.SaveChanges();
            }
        }

   private void SeedTestData()
        {
            
            using (var scope = _server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
               
                dbContext.Database.EnsureCreated();
                if (!dbContext.Products.Any())
                {
                    dbContext.Products.AddRange(new List<Product>
                    {
                        new Product { Name = "Booka", Category = "Categorya", Description = "Descriptiona", Price = 20 },
                        new Product { Name = "Bookb", Category = "Categoryrb", Description = "Descriptionb", Price = 21 }
                    });
                    dbContext.SaveChanges();
                }
            }
        }
        

        [Fact]
        public async Task TestGetProductById()
        {
            // Add test data to the database
            ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            var testProduct = new Product
            {
                Name = "Test Product",
                Category = "Test Category",
                Description = "Test Description",
                Price = 2022
            };
            context.Products.Add(testProduct);
            context.SaveChanges();

            // Perform the GET request to /api/Products/{ProductId}
            var response = await _client.GetAsync($"/api/Products/{testProduct.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response content and verify the result
            var content = await response.Content.ReadAsStringAsync();
            var Product = JsonConvert.DeserializeObject<Product>(content);
            Product.Should().NotBeNull();
            Product.Name.Should().Be(testProduct.Name);
            Product.Category.Should().Be(testProduct.Category);
            Product.Description.Should().Be(testProduct.Description);
            Product.Price.Should().Be(testProduct.Price);
        }

        [Fact]
        public async Task TestDeleteAllProducts()
        {
            // Add test data to the database
            ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            context.Products.Add(new Product
            {
                Name = "Test Product 1",
                Category = "Test Category 1",
                Description = "Test Description 1",
                Price = 2022
            });

            context.Products.Add(new Product
            {
                Name = "Test Product 2",
                Category = "Test Category 2",
                Description = "Test Description 2",
                Price = 2023,
            });

            context.SaveChanges();

            // Perform the DELETE request to /api/Products to delete all Products
            var deleteResponse = await _client.DeleteAsync("/api/Products");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify that all Products were deleted
            var getAllResponse = await _client.GetAsync("/api/Products");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await getAllResponse.Content.ReadAsStringAsync();
            var Products = JsonConvert.DeserializeObject<List<Product>>(content);
            Products.Should().NotBeNull();
            Products.Count.Should().Be(0);
        }


       

    }
}
