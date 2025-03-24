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
        public async Task<IActionResult> GetProducts(
            string? sortBy = "price", // Giá trị mặc định là sắp xếp theo giá
            string? filterCategory = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? rating = null)
        {
            var products = await _productService.GetProductsAsync(sortBy, filterCategory, minPrice, maxPrice, rating);
            return Ok(products);
        }

        // Lấy chi tiết sản phẩm theo ID (tùy chọn nếu cần)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet("getall")]
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productService.GetAll();
        }
    }
}
