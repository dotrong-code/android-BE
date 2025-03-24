using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Repositories.Models;

namespace Services
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task AddToCartAsync(int userId, CartItem cartItem);
        Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(int userId);
        Task<List<CartItem>> GetCartItemList(string userName);
    }
    public class CartService : ICartService
    {
        private readonly CartRepository _cartRepository;
        private readonly CartItemRepository _cartItemRepository;
        private readonly UserRepository _userRepository;

        public CartService(CartRepository cartRepository, CartItemRepository cartItemRepository, UserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userRepository = userRepository;
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, TotalPrice = 0, Status = "Active" };
                await _cartRepository.CreateAsync(cart);
            }
            cart.TotalPrice = cart.CartItems.Sum(item => item.Price * item.Quantity); // Tính tổng giá
            return cart;
        }

        public async Task AddToCartAsync(int userId, CartItem cartItem)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, TotalPrice = 0, Status = "Active" };
                await _cartRepository.CreateAsync(cart);
            }

            cartItem.CartId = cart.CartId;
            await _cartItemRepository.CreateAsync(cartItem);

            cart.TotalPrice = cart.CartItems.Sum(item => item.Price * item.Quantity);
            await _cartRepository.UpdateAsync(cart);
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null || quantity < 1)
                return false;

            cartItem.Quantity = quantity;
            await _cartItemRepository.UpdateAsync(cartItem);

            var cart = await _cartRepository.GetByIdAsync(cartItem.CartId.Value);
            cart.TotalPrice = cart.CartItems.Sum(item => item.Price * item.Quantity);
            await _cartRepository.UpdateAsync(cart);
            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null)
                return false;

            await _cartItemRepository.RemoveAsync(cartItem);

            var cart = await _cartRepository.GetByIdAsync(cartItem.CartId.Value);
            cart.TotalPrice = cart.CartItems.Sum(item => item.Price * item.Quantity);
            await _cartRepository.UpdateAsync(cart);
            return true;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                foreach (var item in cart.CartItems.ToList())
                {
                    await _cartItemRepository.RemoveAsync(item);
                }
                cart.TotalPrice = 0;
                await _cartRepository.UpdateAsync(cart);
            }
        }

        public async Task<List<CartItem>> GetCartItemList(string userName)
        {
            var user = (await _userRepository.GetAllAsync()).FirstOrDefault(x => x.Username.Equals(userName));
            int userId = user.UserId;
            var check = (await _cartItemRepository.GetAllAsync2()).Where(x => x.Cart.UserId == userId).ToList();
            return (List<CartItem>)check;
        }
    }
}
