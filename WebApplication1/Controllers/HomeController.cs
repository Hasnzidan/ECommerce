using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
            SouqcomContext db = new SouqcomContext();

        public IActionResult Index()
        {
            ViewBag.Reviews = db.Review.ToList();
            ViewBag.Products = db.Product.ToList();
            var Cats = db.Category.ToList();
            return View(Cats);
        }

        public IActionResult Item(int id)
        {
            ViewBag.productimages = db.ProductImages.Where(x => x.ProductId == id).ToList();
            var product = db.Product.Where(x => x.Id == id).ToList();
            return View(product);
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult productserach(string Xname)
        {
            var products = db.Product.Where(x => x.Name.Contains(Xname)).ToList();
            return View("Products",products);
        }

        [HttpPost]
            public IActionResult Savereview(Review model)
        {
            db.Review.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
       
       [Authorize]
        public IActionResult Main()
        {

            ViewBag.Products = db.Product.ToList();

            var cats = db.Category.ToList();
            return View(cats);
        }

        public IActionResult Privacy()
        {

            var Cats = db.Category.ToList();
            return View(Cats);
        }

        public IActionResult Products(int id)
        {

           var products =  db.Product.Where(x=>x.Catid == id) .ToList();

            return View(products);
        }
        public IActionResult Cart()
        {
            return View();
        }



       
    }
}
