using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Data;
using System.Security.Claims;

namespace WebApplication1.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly SouqcomContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartViewComponent(SouqcomContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartCount = 0;
            
            if (UserClaimsPrincipal?.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                cartCount = cartItems.Sum(c => c.Qty ?? 0);
            }
            
            return View(cartCount);
        }
    }
}
