using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductImagesController : Controller
    {
        private readonly Data.SouqcomContext _context;

        public ProductImagesController(Data.SouqcomContext context)
        {
            _context = context;
        }

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            var souqcomContext = _context.ProductImages.Include(p => p.Product);
            return View(await souqcomContext.ToListAsync());
        }

        // GET: ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: ProductImages/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: ProductImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId")] ProductImage productImage, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (files == null || !files.Any())
                {
                    ModelState.AddModelError("", "Please select at least one image file.");
                    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
                    return View(productImage);
                }

                var uploadPath = Path.Combine("wwwroot", "uploads", "products");
                Directory.CreateDirectory(uploadPath);

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Validate file type
                        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                        if (!allowedTypes.Contains(file.ContentType.ToLower()))
                        {
                            ModelState.AddModelError("", $"File {file.FileName} is not a valid image type.");
                            continue;
                        }

                        // Validate file size (5MB max)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", $"File {file.FileName} exceeds the maximum size of 5MB.");
                            continue;
                        }

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Create new product image entry
                        var newProductImage = new ProductImage
                        {
                            ProductId = productImage.ProductId,
                            ImagePath = $"uploads/products/{fileName}",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _context.Add(newProductImage);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ImagePath,CreatedAt,UpdatedAt")] ProductImage productImage)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return _context.ProductImages.Any(e => e.Id == id);
        }
    }
}
