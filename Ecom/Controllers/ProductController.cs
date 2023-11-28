using AutoMapper;
using Ecom.Dto;
using Ecom.Interfaces;
using Ecom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Uncomment this line if you want to restrict access to Admin role
    public class ProductController : ControllerBase
    {
        private readonly IProductInterface _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductInterface productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetProducts()
        {
            var products = _mapper.Map<List<ProductDto>>(_productRepository.GetAllProducts());
            return Ok(products);
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(200, Type = typeof(ProductDto))]
        [ProducesResponseType(400)]
        public IActionResult GetProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var product = _mapper.Map<ProductDto>(_productRepository.GetProductById(productId));
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(201)] // Created
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(422)] // Unprocessable Entity
        [ProducesResponseType(500)] // Internal Server Error
        public IActionResult CreateProduct([FromQuery] int categoryId, [FromBody] ProductDto productCreate)
        {
            if (productCreate == null)
                return BadRequest(ModelState);

            // Validate that the required properties are provided
            if (string.IsNullOrEmpty(productCreate.Name))
            {
                ModelState.AddModelError("Name", "The product name is required.");
                return BadRequest(ModelState);
            }

            // Check for duplicate product names
            var existingProduct = _productRepository.GetAllProducts()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == productCreate.Name.TrimEnd().ToUpper());

            if (existingProduct != null)
            {
                ModelState.AddModelError("", "Product already exists");
                return UnprocessableEntity(ModelState);
            }

            // Other validation logic...

            // If the ModelState is valid, proceed with creating the product
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productMap = _mapper.Map<Product>(productCreate);
            productMap.Category = _categoryRepository.GetCategory(categoryId);

            if (!_productRepository.AddProduct(productMap, categoryId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return CreatedAtAction(nameof(GetProduct), new { productId = productMap.Id }, "Successfully created");
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductDto updateProduct)
        {
            if (updateProduct == null || productId != updateProduct.Id)
                return BadRequest(ModelState);

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            // Check for duplicate product name
            if (_productRepository.ProductNameExists(updateProduct.Name.TrimEnd(), updateProduct.Id))
            {
                ModelState.AddModelError("", "Product name already exists in this category");
                return UnprocessableEntity(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productMap = _mapper.Map<Product>(updateProduct);

            if (!_productRepository.UpdateProduct(productMap))
            {
                ModelState.AddModelError("", "Something went wrong updating product");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpDelete("{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(404)]
        public IActionResult DeleteProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete = _productRepository.GetProductById(productId);

            if (!_productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting product");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
