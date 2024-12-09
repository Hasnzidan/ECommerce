using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly WebApplication1.Data.SouqcomContext db;

        public ProductsController(WebApplication1.Data.SouqcomContext context)
        {
            db = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var souqcomContext = db.Products.Include(p => p.Category);
            return View(await souqcomContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.productimages = db.ProductImages.Where(x => x.ProductId == id).ToList();
            var product = await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CategoryId,Photo")] Product product, IFormFile Photo)
        {
            if (ModelState.IsValid)
            {
                product.IsAvailable = Request.Form["IsAvailable"].ToString() == "true";
                product.CreatedAt = DateTime.Now;
                
                if (Photo != null && Photo.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    product.Photo = Path.Combine("uploads", "products", uniqueFileName).Replace("\\", "/");
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryId,Photo,IsAvailable")] Product product, IFormFile Photo)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Keep the existing photo if no new photo is uploaded
                    if (Photo == null)
                    {
                        product.Photo = existingProduct.Photo;
                    }
                    else if (Photo.Length > 0)
                    {
                        // Delete old photo if it exists
                        if (!string.IsNullOrEmpty(existingProduct.Photo))
                        {
                            var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.Photo);
                            if (System.IO.File.Exists(oldPhotoPath))
                            {
                                System.IO.File.Delete(oldPhotoPath);
                            }
                        }

                        // Save new photo
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Photo.CopyToAsync(fileStream);
                        }
                        product.Photo = Path.Combine("uploads", "products", uniqueFileName).Replace("\\", "/");
                    }

                    product.IsAvailable = Request.Form["IsAvailable"].ToString() == "true";
                    db.Update(product);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.productimages = db.ProductImages.Where(x => x.ProductId == id).ToList();
            var product = await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return db.Products.Any(e => e.Id == id);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Please login to add items to cart" });
                }

                var product = await db.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                var cartItem = await db.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Qty = quantity;
                    db.Carts.Update(cartItem);
                }
                else
                {
                    cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = productId,
                        Qty = quantity
                    };
                    db.Carts.Add(cartItem);
                }

                await db.SaveChangesAsync();

                var cartCount = await db.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Qty);

                return Json(new { success = true, message = "Item added to cart", cartCount });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error adding item to cart" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = true, count = 0 });
                }

                var count = await db.Carts
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Qty);

                return Json(new { success = true, count });
            }
            catch (Exception)
            {
                return Json(new { success = false, count = 0 });
            }
        }
    }
}
