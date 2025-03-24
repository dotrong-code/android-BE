using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;

        public CartController(ICartService cartService, IProductService productService, INotificationService notificationService)
        {
            _cartService = cartService;
            _productService = productService;
            _notificationService = notificationService;
        }

        // Xem giỏ hàng
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
                return NotFound("Cart not found");
            return Ok(cart);
        }

        // Cập nhật số lượng sản phẩm trong giỏ
        [HttpPut("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var success = await _cartService.UpdateCartItemQuantityAsync(request.CartItemId, request.Quantity);
            if (!success)
                return NotFound("Cart item not found");
            return Ok("Quantity updated successfully");
        }

        // Xóa sản phẩm khỏi giỏ
        [HttpDelete("RemoveItem/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var success = await _cartService.RemoveCartItemAsync(cartItemId);
            if (!success)
                return NotFound("Cart item not found");
            return Ok("Item removed from cart");
        }

        // Xóa toàn bộ giỏ hàng
        [HttpDelete("ClearCart/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared successfully");
        }

        // Lấy số lượng sản phẩm trong giỏ hàng (dành cho badge)
        [HttpGet("ItemCount/{userId}")]
        public async Task<IActionResult> GetCartItemCount(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
                return Ok(0);
            var itemCount = cart.CartItems.Sum(item => item.Quantity);
            return Ok(itemCount);
        }

        // Thêm sản phẩm vào giỏ và gửi thông báo
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null)
                return NotFound("Product not found");

            var cartItem = new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.Price
            };

            await _cartService.AddToCartAsync(request.UserId, cartItem);

            // Gửi thông báo khi thêm sản phẩm vào giỏ
            var cart = await _cartService.GetCartByUserIdAsync(request.UserId);
            var itemCount = cart.CartItems.Sum(item => item.Quantity);
            await _notificationService.SendCartUpdateNotification(request.UserId, itemCount);

            return Ok("Product added to cart successfully");
        }

        public sealed record AddToCartRequest(int UserId, int ProductId, int Quantity);
        public sealed record UpdateCartRequest(int CartItemId, int Quantity);
    }
}
