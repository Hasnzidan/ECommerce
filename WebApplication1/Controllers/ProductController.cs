using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly SouqcomContext db = new SouqcomContext();

        public IActionResult Index()
        {
            var products = db.Product.ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            ViewBag.ProductImages = db.ProductImages.Where(x => x.ProductId == id).ToList();
            var product = db.Product.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Search(string query)
        {
            var products = db.Product.Where(x => x.Name.Contains(query)).ToList();
            return View("Index", products);
        }

        public IActionResult Category(int id)
        {
            var products = db.Product.Where(x => x.CategoryId == id).ToList();
            return View("Index", products);
        }
    }
}
