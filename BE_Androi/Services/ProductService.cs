using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;

namespace Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync(string sortBy, string filterCategory, decimal? minPrice, decimal? maxPrice, int? rating);
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAll();
    }
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetProductsAsync(string sortBy, string filterCategory, decimal? minPrice, decimal? maxPrice, int? rating)
        {
            var products = await _productRepository.GetAllProductsAsync();

            // Lọc sản phẩm
            if (!string.IsNullOrEmpty(filterCategory))
                products = products.Where(p => p.Category != null && p.Category.CategoryName == filterCategory).ToList();
            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            // Nếu có thêm thuộc tính rating trong Product, bạn có thể lọc theo rating
            // if (rating.HasValue)
            //     products = products.Where(p => p.Rating >= rating.Value).ToList();

            // Sắp xếp sản phẩm
            switch (sortBy.ToLower())
            {
                case "price":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "popularity":
                    // Giả sử popularity dựa trên số lượng bán hoặc một trường khác, hiện tại để mặc định
                    products = products.OrderBy(p => p.ProductId).ToList(); // Ví dụ
                    break;
                case "category":
                    products = products.OrderBy(p => p.Category?.CategoryName).ToList();
                    break;
                default:
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync2(id);
        }
        public async Task<List<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }
    }
}
     

