using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SouqcomContext _context;
        private readonly int _pageSize = 8;

        public HomeController(ILogger<HomeController> logger, SouqcomContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageProduct = 1, int? pageCategory = 1)
        {
            try
            {
                var productsQuery = _context.Products
                    .Include(p => p.ProductImage)
                    .Include(p => p.Category);

                var categoriesQuery = _context.Categories;

                var viewModel = new PaginatedListViewModel
                {
                    Products = await PaginatedList<Product>.CreateAsync(
                        productsQuery, pageProduct ?? 1, _pageSize),
                    Categories = await PaginatedList<Category>.CreateAsync(
                        categoriesQuery, pageCategory ?? 1, _pageSize)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products and categories");
                return View(new PaginatedListViewModel
                {
                    Products = new PaginatedList<Product>(new List<Product>(), 0, 1, _pageSize),
                    Categories = new PaginatedList<Category>(new List<Category>(), 0, 1, _pageSize)
                });
            }
        }

        // GET: /Home/Categories - Shows all categories
        public IActionResult Categories()
        {
            var categories = _context.Categories
                .Include(c => c.Products)
                .ToList();
            return View(categories);
        }

        // GET: /Home/Category/5 - Shows products in a specific category
        public IActionResult Category(int? categoryId)
        {
            IQueryable<Product> query = _context.Products;

            // First include the Category
            query = query.Include(p => p.Category);

            // Then include the ProductImage collection
            query = query.Include(p => p.ProductImage);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
                var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    ViewBag.CategoryName = category.Name;
                    ViewBag.CategoryDescription = category.Description;
                }
            }

            return View(query.ToList());
        }

        public IActionResult Products(int? categoryId)
        {
            IQueryable<Product> products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImage);

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }

            return View(products.ToList());
        }

        public IActionResult Item(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImage)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Here you would add the item to the cart
                // For now, we'll just return success
                return Json(new { success = true, cartCount = 1 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return Json(new { success = false, message = "Error adding item to cart" });
            }
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Products));
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImage)
                .Where(p => p.Name.Contains(query) || 
                           p.Description.Contains(query) || 
                           p.Category.Name.Contains(query))
                .ToList();

            ViewBag.SearchQuery = query;
            return View("Products", products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
