using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MainIntegrationTests.Tests
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
        public async Task TestYourIntegrationScenario()
        {
            ClearTasksTable();
            // For example:
            var response = await _client.GetAsync("/api/products");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
        }

        [Fact]
        public async Task TestAddProduct()
        {
            ClearTasksTable();
            var newProduct = new Product
            {
                Name = "Sample Product",
                Description = "Sample Description",
                Category = "Fruits",
                Price = 50,
               
            };

            var jsonString = JsonConvert.SerializeObject(newProduct);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/products", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task TestUpdateProduct()
        {
          ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            context.Products.Add(new Product
            {
                Name = "Product",
                Description = "Description",
                Category = "Fruits",
                Price = 20,
               
            });
            context.SaveChanges();

            var response = await _client.GetAsync("/api/products");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
            products.Count.Should().Be(1);
            var productId = products[0].Id;

            // Update the Product
            var updatedProduct = new Product
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Description",
                Category = "Updated Category",
                Price = 21,
            };
            var jsonString = JsonConvert.SerializeObject(updatedProduct);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var updateResponse = await _client.PutAsync($"/api/products/{productId}", httpContent);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the Product was updated
            var updatedResponse = await _client.GetAsync($"/api/products/{productId}");
            updatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedContent = await updatedResponse.Content.ReadAsStringAsync();
            var updatedProductResult = JsonConvert.DeserializeObject<Product>(updatedContent);
            updatedProductResult.Should().NotBeNull();
            updatedProductResult.Name.Should().Be("Updated Product");
            updatedProductResult.Category.Should().Be("Updated Category");
            updatedProductResult. Description.Should().Be("Description");
            updatedProductResult.Price.Should().Be(21);
        }

        [Fact]
        public async Task TestGetProductByName()
        {
          ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            var testProduct = new Product
            {
                Name = "Test Product",
                Category = "Test Category",
                Description = "Test",
                Price = 2022,
            };
            context.Products.Add(testProduct);
            context.SaveChanges();

            // Perform the GET request to /api/products?Name={ProductName}
            var response = await _client.GetAsync($"/api/products?Name={testProduct.Name}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response content and verify the result
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
            products.Count.Should().Be(1);

            var Product = products.FirstOrDefault();
            Product.Should().NotBeNull();
            Product.Name.Should().Be(testProduct.Name);
            Product.Category.Should().Be(testProduct.Category);
            Product.Description.Should().Be(testProduct.Description);
            Product.Price.Should().Be(testProduct.Price);
           
        }

        [Fact]
        public async Task TestGetAllProducts()
        {
            ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            context.Products.Add(new Product
            {
                Name = "Test Product",
                Category = "Test Category",
                Description = "Test",
                Price = 2022,
               
            });
            context.SaveChanges();

            // Perform the GET request to /api/products
            var response = await _client.GetAsync("/api/products");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response content and verify the results
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
            products.Count.Should().Be(1);

            var testProduct = products.FirstOrDefault();
            testProduct.Should().NotBeNull();
            testProduct.Name.Should().Be("Test Product");
            testProduct.Category.Should().Be("Test Category");
            testProduct. Description.Should().Be("Test");
            testProduct.Price.Should().Be(2022);
        }

        [Fact]
        public async Task TestDeleteProduct()
        {
           ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            context.Products.Add(new Product
            {
                Name = "Test Product",
                Category = "Test Category",
                Description = "Test  Description",
                Price = 2022,
            });
            context.SaveChanges();

            // Perform the GET request to /api/products to get the Product ID
            var response = await _client.GetAsync("/api/products");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
            products.Count.Should().Be(1);
            var ProductId = products[0].Id;

            // Delete the Product
            var deleteResponse = await _client.DeleteAsync($"/api/products/{ProductId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify the Product was deleted
            var deletedResponse = await _client.GetAsync($"/api/products/{ProductId}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

     
        [Fact]
        public async Task TestGetTotalProductCount()
        {
          ClearTasksTable();

            // Act: Send a GET request to the API
            var response = await _client.GetAsync("/api/products/total-count");

            // Assert: Check the response status code
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Deserialize the response content as an integer
            var content = await response.Content.ReadAsStringAsync();
            var totalCount = JsonConvert.DeserializeObject<int>(content);

            // Assert: Check the total count
            totalCount.Should().BeGreaterOrEqualTo(0);

            // You can add more specific assertions based on your application's logic
        }

        [Fact]
        public async Task TestGetProductsByCategory()
        {
           ClearTasksTable();
            var context = _server.Host.Services.GetRequiredService<ProductContext>();
            context.Products.Add(new Product
            {
                Name = "Product1",
                Category = "Category1",
                Description = "Description1",
                Price = 10.0m,
            });
            context.Products.Add(new Product
            {
                Name = "Product2",
                Category = "Category2",
                Description = "Description2",
                Price = 20.0m,
            });
            context.SaveChanges();
        
            // Act: Send a GET request to the API to retrieve products by category
            var response = await _client.GetAsync("/api/products/category/Category1");
        
            // Assert: Check the response status code
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        
            // Deserialize the response content as a list of products
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(content);
            products.Should().NotBeNull();
        
            // Assert: Check that the correct product(s) matching the category are returned
            products.Count.Should().Be(1);
            var product = products[0];
            product.Name.Should().Be("Product1");
            product.Category.Should().Be("Category1");
            product.Description.Should().Be("Description1");
            product.Price.Should().Be(10.0m);
        
            // You can add more assertions as needed based on your specific requirements
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
