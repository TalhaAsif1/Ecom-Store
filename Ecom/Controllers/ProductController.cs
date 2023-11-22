using AutoMapper;
using Ecom.Dto;
using Ecom.Interfaces;
using Ecom.Models;
using Ecom.Repositoy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
            private readonly IProductInterface _productRepository;
            private readonly ICategoryRepository _categoryRepository;
            private readonly IMapper _mapper;

        public ProductController(IProductInterface productRepository, IMapper mapper, ICategoryRepository categoryRepository)
            {
             _productRepository = productRepository;
             _categoryRepository = categoryRepository;
             _mapper=mapper;
            }

            [HttpGet, AllowAnonymous]
            [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
            public IActionResult GetProducts()
            {
                var products = _mapper.Map<List<ProductCategoryDto>>(_productRepository.GetAllProducts());
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(products);
            }


            [HttpGet("{productId}"), AllowAnonymous]
            [ProducesResponseType(200, Type = typeof(Product))]
            [ProducesResponseType(400)]
            public IActionResult GetProduct(int productId)
            {
                if (!_productRepository.ProductExists(productId))
                    return NotFound();

                var product = _mapper.Map<ProductCategoryDto>(_productRepository.GetProductById(productId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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

            var existingProduct = _productRepository.GetAllProducts()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == productCreate.Name.TrimEnd().ToUpper());

            if (existingProduct != null)
            {
                ModelState.AddModelError("", "Product already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productMap = _mapper.Map<Product>(productCreate);
            productMap.Category = _categoryRepository.GetCategory(categoryId);

            if (!_productRepository.AddProduct(productMap, categoryId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(404)]
        public IActionResult UpdateProduct(int productId, [FromBody] ProductDto updateProduct)
        {
            if (updateProduct == null)
                return BadRequest(ModelState);

            if (productId != updateProduct.Id)
                return BadRequest(ModelState);

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

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
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(404)]

        public IActionResult DeleteProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete =_productRepository.GetProductById(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting product");
            }
            return NoContent();
        }


    }
}
