using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Extensions;
using StoreApi.Models;


namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly PrintStoreDbContext _context;
        public CartController(PrintStoreDbContext context)
        {
            _context = context;

        }

        [HttpGet(Name = "GetCart")]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var cart = await RetrieveCart(GetCustomerId());

            if (cart == null) return NotFound();

            return cart.MapCartToDTO();
        }


        [HttpPost]
        public async Task<ActionResult<CartDTO>> AddItemToCart(int productId, int quantity)
        {
            var cart = await RetrieveCart(GetCustomerId());
            if (cart == null) cart = CreateCart();

            //get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = "Product not found" });

            // add item
            // get quantity from query string
            cart.AddItem(product, quantity);

            // save changes
            // SaveChangeAsync method returns integer (number) of changes that have been made in our database
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetCart", cart.MapCartToDTO());

            return BadRequest(new ProblemDetails { Title = "Problem saving item to cart" });
        }


        [HttpDelete]
        public async Task<ActionResult> RemoveCartItem(int productId, int quantity)
        {
            var cart = await RetrieveCart(GetCustomerId());

            if (cart == null)
                return NotFound();

            // remove item or reduce quantity
            cart.DeleteItem(productId, quantity);

            // save changes
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Problem removing item to cart" });
        }

        private async Task<Cart> RetrieveCart(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                Response.Cookies.Delete("customerId");
                return null;
            }
            return await _context.Carts
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        private string GetCustomerId()
        {
            return User.Identity?.Name ?? Request.Cookies["customerId"];
        }
        private Cart CreateCart()
        {
            var customerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(customerId))
            {
                customerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("customerId", customerId, cookieOptions);
            }
            var cart = new Cart { CustomerId = customerId };
            _context.Carts.Add(cart);
            return cart;
        }
    }
}
