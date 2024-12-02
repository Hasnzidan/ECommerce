using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
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
                var userId = _userManager.GetUserId((ClaimsPrincipal)UserClaimsPrincipal);
                if (!string.IsNullOrEmpty(userId))
                {
                    cartCount = await _context.Cart
                        .Where(c => c.UserId == userId)
                        .Select(c => c.Qty ?? 0)
                        .SumAsync();
                }
            }
            
            return View(cartCount);
        }
    }
}
