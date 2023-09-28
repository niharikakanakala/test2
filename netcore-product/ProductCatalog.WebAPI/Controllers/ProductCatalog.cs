using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.WebAPI.Models;
using ProductCatalog.WebAPI.Services;

namespace ProductCatalog.WebAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /*
        Post api/products: This endpoint allows adding a new product, asynchronously invoking _productService.AddProduct, 
        and returning an HTTP 201 Created status.
        */

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody]Product product)
        {
            if(product == null)
            {
                return BadRequest();
            }

            await _productService.AddProduct(product);

            return StatusCode(201);

        }        
        
        /*
         GET api/products: When a client sends a GET request to this endpoint, it asynchronously retrieves a list of products using the _productService, 
        and then returns those products as a response with an HTTP status code indicating success (200 OK).
        */

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = await _productService.GetAllProducts();
            
            return Ok(products);
        }
        
        /*
        GET api/products/{id}: This endpoint that handles HTTP GET requests with a specific "id" parameter, retrieving a product by its unique identifier asynchronously. 
        If the product is found, it returns it with an HTTP 200 OK status; otherwise, it returns an HTTP 404 Not Found status.
        */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }
        
        /*
        GET api/products/search: This endpoint accepts a "name" query parameter, asynchronously retrieves products with matching names 
        using _productService.GetProductsByName, and returns them in an HTTP response.
        */
        [HttpGet("search")]
        public async Task<IActionResult> GetProductByNameAsync([FromQuery] string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            var products = await _productService.GetProductsByName(name);

            if(products.Count == 0)
            {
                return NoContent();
            }

            return Ok(products);
        }
        
        /*
        GET api/products/total-count: This endpoint asynchronously retrieves the total count of products and returns it in an HTTP response,
        handling potential exceptions with a 500 Internal Server Error status.
        */
        [HttpGet("total-count")]        
        public async Task<IActionResult> GetTotalProductCountAsync()
        {
            try
            {
                var totalCount = await _productService.GetTotalProductCount();
                return Ok(totalCount);                
            }
            catch(Exception ex)
            {
                return StatusCode(500, "internal error");
            }
        }
       
        /*
        PUT api/products/{id}: This endpoint handles HTTP PUT requests to update a product by its unique identifier. 
        It checks if the provided product ID matches the one in the request body and if the product exists; 
        if so, it updates the product's attributes and returns a 204 No Content status to indicate success.
        */

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, Product updatedProduct)
        {
            if(updatedProduct == null)
            {
                return BadRequest("Product data is empty");
            }

            if(id != updatedProduct.Id)
            {
                return BadRequest("Product Id does not match");
            }

            
            try
            {
                await _productService.UpdateProduct(updatedProduct);
                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, "internal error");
            }

        }
        
       /*
       GET api/products/sort: This endpoint allows sorting products based on specified criteria (name, category, or price) and 
       order (ascending or descending), returning the sorted list in an HTTP response, with an option to specify the sorting order.
       */

       [HttpGet("sort")]
       public async Task<IActionResult> GetSortedProductsAsync([FromQuery] string sortBy, [FromQuery] string sortOrder)
       {
           try
           {
               List<Product> sortedProducts;
                switch (sortBy.ToLower())
                {
                    case "name":
                        sortedProducts = await _productService.SortProductsByName(sortOrder);
                        break;
                    case "category":
                        sortedProducts = await _productService.SortProductsByCategory(sortOrder);
                        break;
                    case "price":
                        sortedProducts = await _productService.SortProductsByPrice(sortOrder);
                        break;
                    default:
                        return BadRequest("Invalid sort criteria");

                }

                return Ok(sortedProducts);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "internal error");
            }

        }
       
       
       /*
       GET api/products/category/{category}: This endpoint asynchronously retrieves products by a specified category, 
       handling potential exceptions and returning them in an HTTP response, with a 404 status if no products are found for the category.
       */

       [HttpGet("category/{category}")]
       public async Task<IActionResult> GetProductsByCategoryAsync(string category)
       {
           try
           {
               var products = await _productService.GetProductsByCategory(category);
               if(products.Count == 0)
               {
                   return StatusCode(404, "No products found");
               }
               return Ok(products);
           }
           catch(Exception ex)
           {
               return StatusCode(500, "internal error");
           }
       }
       
       /*
       DELETE api/products/{id}: This endpoint handles HTTP DELETE requests to delete a product by its unique identifier,
       returning a 204 No Content status upon successful deletion or a 404 status if the product is not found.
       */

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductByIdAsync(int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if(product == null)
                {
                    return NotFound();
                }
                await _productService.DeleteProduct(id);
                return NoContent();

            }
            catch(Exception ex)
            {
                return StatusCode(500, "internal error");
            }
        }
       
      /*
      DELETE api/products: This endpoint handles HTTP DELETE requests to delete all products, returning a 204 No Content status upon successful deletion.
      */
        
        [HttpDelete]
        public async Task<IActionResult> DeleteAllProductsAsync()
        {
            try
            {
                await _productService.DeleteAllProducts();
                return NoContent();

            }
            catch(Exception ex)
            {
                return StatusCode(500, "internal error");
            }
        }

    }   

    
    
}
