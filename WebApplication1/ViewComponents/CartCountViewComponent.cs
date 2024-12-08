using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Linq;
using WebApplication1.Data;

namespace WebApplication1.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly WebApplication1.Data.SouqcomContext _context;

        public CartCountViewComponent(WebApplication1.Data.SouqcomContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Content("0");
            }

            var count = await _context.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Qty ?? 0);

            return Content(count.ToString());
        }
    }
}
