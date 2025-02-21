using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class PaginatedListViewModel
    {
     public PaginatedList<Product> Products { get; set; }
     public PaginatedList<Category> Categories { get; set; }
    }
}