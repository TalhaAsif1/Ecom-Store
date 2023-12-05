using AutoMapper;
using DotNetOpenAuth.InfoCard;
using Ecom.Dto;
using Ecom.Interfaces;
using Ecom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]

    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public Task<IActionResult> GetCategories()
        {
            var userTypeClaim = HttpContext.User.FindFirst("UserType");

            var isAdmin = HttpContext.User.IsInRole("Admin");

            if (isAdmin || (userTypeClaim != null && userTypeClaim.Value == "Regular"))
            {
                var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

                if (!ModelState.IsValid)
                    return Task.FromResult<IActionResult>(BadRequest(ModelState));

                return Task.FromResult<IActionResult>(Ok(categories));
            }

            return Task.FromResult<IActionResult>(Unauthorized(new { Message = "User not authorized to view categories." }));
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var userTypeClaim = HttpContext.User.FindFirst("UserType");

            var isAdmin = HttpContext.User.IsInRole("Admin");

            if (isAdmin || (userTypeClaim != null && userTypeClaim.Value == "Premium"))
            {
                var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(category);
            }

            // Unauthorized for other user types
            return Unauthorized(new { Message = "User not authorized to view this category." });
        }



        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if (categoryCreate == null)
                return BadRequest(ModelState);

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryCreate);

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }
        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updateCategory)
        {
            if (updateCategory == null)
                return BadRequest(ModelState);

            if (categoryId != updateCategory.Id)
               return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var categoryMap = _mapper.Map<Category>(updateCategory);

            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]//no content
        [ProducesResponseType(404)]

        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }
            return NoContent();
        }

        [HttpGet("{categoryId}/products")]
        public IActionResult GetProductsInCategory(int categoryId)
        {
            // Check if the category exists
            var category = _categoryRepository.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound($"Category with ID {categoryId} not found");
            }

            // Check if there are products in the category
            var products = _categoryRepository.GetProductByCategory(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"No products found in category with ID {categoryId}");
            }

            var productDtos = new List<ProductInCategoryDto>();
            foreach (var product in products)
            {
                var productDto = new ProductInCategoryDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = product.Quantity,
                    CategoryId = product.Category.Id,
                    CategoryName = product.Category.Name,
                    CategoryDescription = product.Category.Description
                };

                productDtos.Add(productDto);
            }

            return Ok(productDtos);
        }

    }
}