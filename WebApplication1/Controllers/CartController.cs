using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly SouqcomContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(SouqcomContext context, UserManager<IdentityUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var cartItems = await _db.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int change)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _db.Carts
                    .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                var newQuantity = cartItem.Qty + change;
                if (newQuantity <= 0)
                {
                    _db.Carts.Remove(cartItem);
                }
                else if (newQuantity <= 99)
                {
                    cartItem.Qty = newQuantity;
                    _db.Carts.Update(cartItem);
                }
                else
                {
                    return Json(new { success = false, message = "Maximum quantity is 99" });
                }

                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update quantity" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            try
            {
                if (model.Quantity <= 0 || model.Quantity > 99)
                {
                    return Json(new { success = false, message = "Invalid quantity" });
                }

                var userId = GetUserId();
                var product = await _db.Products.FindAsync(model.ProductId);
                
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                var existingItem = await _db.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == model.ProductId);

                if (existingItem != null)
                {
                    var newQuantity = existingItem.Qty + model.Quantity;
                    if (newQuantity > 99)
                    {
                        return Json(new { success = false, message = "Maximum quantity is 99" });
                    }
                    existingItem.Qty = newQuantity;
                    _db.Carts.Update(existingItem);
                }
                else
                {
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = model.ProductId,
                        Qty = model.Quantity
                    };
                    await _db.Carts.AddAsync(cartItem);
                }

                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to add item to cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = await _db.Carts
                    .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                _db.Carts.Remove(cartItem);
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to remove item from cart" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = GetUserId();
            var count = await _db.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Qty);
            return Json(count);
        }
    }

    public class AddToCartModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
