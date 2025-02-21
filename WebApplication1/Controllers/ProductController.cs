using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly SouqcomContext _db;

        public ProductController(SouqcomContext context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            ViewBag.ProductImages = _db.ProductImages.Where(x => x.ProductId == id).ToList();
            var product = _db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Search(string query)
        {
            var products = _db.Products.Where(x => x.Name.Contains(query)).ToList();
            return View("Index", products);
        }

        public IActionResult Category(int id)
        {
            var products = _db.Products.Where(x => x.CategoryId == id).ToList();
            return View("Index", products);
        }
    }
}
