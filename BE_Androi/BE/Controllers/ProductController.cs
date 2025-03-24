using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Lấy danh sách sản phẩm với tùy chọn sắp xếp và lọc
        [HttpGet]
        public async Task<object?> GetProducts(
            string? sortBy = "price", // Giá trị mặc định là sắp xếp theo giá
            string? filterCategory = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? rating = null)
        {
            var products = await _productService.GetProductsAsync(sortBy, filterCategory, minPrice, maxPrice, rating);
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                BriefDescription = p.BriefDescription,
                FullDescription = p.FullDescription,
                TechnicalSpecifications = p.TechnicalSpecifications,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = (int)p.CategoryId,
                CategoryName = p.Category?.CategoryName  // Map the category name if needed
            }).ToList();

            return Ok(productDtos);
        }

        // Lấy chi tiết sản phẩm theo ID (tùy chọn nếu cần)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            var res = new
            {

                ProductId = product.ProductId,
                ProductName = product.ProductName,
                BriefDescription = product.BriefDescription,
                FullDescription = product.FullDescription,
                TechnicalSpecifications = product.TechnicalSpecifications,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.CategoryName
            };
            if (product == null)
                return NotFound();
            return Ok(res);
        }

        [HttpGet("getall")]
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productService.GetAll();
        }
    }

    class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BriefDescription { get; set; }
        public string FullDescription { get; set; }
        public string TechnicalSpecifications { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }  // Optional: If you need the category name instead of ID
    }

}
