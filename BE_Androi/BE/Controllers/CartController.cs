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
        [HttpGet("cartItems/{userName}")]
        public async Task<IActionResult> GetCartItemList(string userName)
        {
            var cart = await _cartService.GetCartItemList(userName);
            var res = cart.Select(x => MapToDTO(x)).ToList();
            return Ok(res);

        }
        private CartItemListDTO MapToDTO(CartItem cartItem)
        {
            // Assuming Product is a navigation property in CartItem
            return new CartItemListDTO
            {
                CartItemId = cartItem.CartItemId,
                ProductId = cartItem.ProductId ?? 0, // Null-coalescing in case ProductId is nullable
                ProductName = cartItem.Product?.ProductName
                 ?? string.Empty, // Assuming Product has Name property
                Price = (double)cartItem.Price, // Convert from decimal to double
                Quantity = cartItem.Quantity,
                ProductImage = cartItem.Product?.ImageUrl ?? string.Empty // Assuming Product has ImageUrl property
            };
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
        [HttpDelete("ClearCart/{userName}")]
        public async Task<IActionResult> ClearCart(string userName)
        {
            await _cartService.ClearCartAsync(userName);
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

            await _cartService.AddToCartAsync(request.UserName, cartItem);

            // Gửi thông báo khi thêm sản phẩm vào giỏ
            //var cart = await _cartService.GetCartByUserIdAsync(request.UserName);
            //var itemCount = cart.CartItems.Sum(item => item.Quantity);
            //await _notificationService.SendCartUpdateNotification(request.UserName, itemCount);

            return Ok("Product added to cart successfully");
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string UserName { get; set; }
        }
        public class CartItemListDTO
        {
            public int CartItemId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public string ProductImage { get; set; }
        }
        public sealed record UpdateCartRequest(int CartItemId, int Quantity);
    }
}
