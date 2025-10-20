using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.BLL.IServices;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.Models.Models;

namespace WebBanHang.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddToCart(string userId, int foodId, int quantity)
        {
            //lay ra gio hang cua user
            var cart = await GetCartByUserId(userId);
            // kiem tra mon an co ton tai khong
            var food = await _unitOfWork.Foods.GetByIdAsync(foodId);
            if (food == null || !food.IsAvailable)
            {
                throw new ArgumentException("Invalid food item.");
            }
            // kiem tra mon an da co trong gio hang chua
            var existingCartItem = await _unitOfWork.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.FoodId == foodId);
            if (existingCartItem == null)
            {

                var cartItem = new CartItem
                {
                    CartItemId = cart.CartItems.Count > 0 ? cart.CartItems.Max(ci => ci.CartItemId) + 1 : 1,
                    CartId = cart.CartId,
                    FoodId = foodId,
                    Quantity = quantity,
                    Price = food.Price,
                    AddedAt = DateTime.UtcNow,

                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }
            else
            {
                existingCartItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(existingCartItem);
            }
            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.SaveAsync();
        }


        public async Task ClearCart(string userId)
        {
            var cart = await _unitOfWork.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
            {
                var cartItems = await _unitOfWork.CartItems.FindAsync(c => c.CartId == cart.CartId);
                foreach (var item in cartItems)
                {
                    await _unitOfWork.CartItems.DeleteAsync(item.CartItemId);
                }
                cart.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Carts.Update(cart);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<Cart> GetCartByUserId(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }
            cart.CartItems = _unitOfWork.CartItems.GetAllQueryable().Include("Food").Where(c => c.CartId == cart.CartId).ToList();
            return cart;
        }
        public Task<Cart> GetCartByUserIdAsync(string userId)
        => GetCartByUserId(userId);
        public async Task<int> GetCartItemCount(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return 0;
            }
            return await _unitOfWork.CartItems.Count(c => c.CartId == cart.CartId);
        }

        public async Task<decimal> GetCartTotal(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFromCart(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem != null)
            {
                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                await _unitOfWork.CartItems.DeleteAsync(cartItemId);
                cart.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Carts.Update(cart);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task UpdateCartItem(int cartItemId, int quantity)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem != null)
            {
                if (cartItem.Quantity <= 0)
                {
                    RemoveFromCart(cartItemId);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    _unitOfWork.CartItems.Update(cartItem);
                    var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                    cart.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Carts.Update(cart);
                    await _unitOfWork.SaveAsync();
                }
            }
        }
    }
}