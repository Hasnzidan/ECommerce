using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly SouqcomContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(SouqcomContext context, UserManager<IdentityUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _userManager.GetUserId(User);
        }

        public IActionResult Index()
        {
            var userId = GetUserId();
            var cartItems = db.Cart
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList();
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int cartId, int change)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = db.Cart.FirstOrDefault(c => c.Id == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                var newQuantity = (cartItem.Qty ?? 0) + change;
                if (newQuantity <= 0)
                {
                    db.Cart.Remove(cartItem);
                }
                else
                {
                    cartItem.Qty = newQuantity;
                    db.Cart.Update(cartItem);
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = GetUserId();
                var existingItem = db.Cart.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Qty = (existingItem.Qty ?? 0) + quantity;
                    db.Cart.Update(existingItem);
                }
                else
                {
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = productId,
                        Qty = quantity
                    };
                    db.Cart.Add(cartItem);
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int cartId)
        {
            try
            {
                var userId = GetUserId();
                var cartItem = db.Cart.FirstOrDefault(c => c.Id == cartId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                db.Cart.Remove(cartItem);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
