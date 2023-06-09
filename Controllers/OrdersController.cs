using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTO;
using StoreApi.Extensions;
using StoreApi.Models;
using StoreApi.Models.Order;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly PrintStoreDbContext _context;
        public OrdersController(PrintStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> GetOrders()
        {
            return await _context.Orders
            .ProjectOrderToOrderDTO()
            .Where(x => x.CustomerId == User.Identity.Name)
            .ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            return await _context.Orders
            .ProjectOrderToOrderDTO()
            .Where(x => x.Id == id && x.CustomerId == User.Identity.Name)
            .FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDTO orderDTO)
        {
            var cart = await _context.Carts.RetrieveCartWithItems(User.Identity.Name)
            .FirstOrDefaultAsync();

            if (cart == null) return BadRequest(new ProblemDetails { Title = "Could not locate cart" });

            var items = new List<OrderItem>();
            foreach (var item in cart.Items)
            {
                var productItem = await _context.Products.FindAsync(item.ProductId);

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };

                items.Add(orderItem);
                productItem.QuantityInStock -= item.Quantity;
            }

            var subtotal = items.Sum(item => item.Price * item.Quantity);
            var deliveryFee = subtotal > 10000 ? 0 : 500;

            var order = new Order
            {
                OrderItems = items,
                ShippingAddress = orderDTO.ShippingAddress,
                CustomerId = User.Identity.Name,
                SubTotal = subtotal,
                DeliveryFee = deliveryFee,
                PaymentIntentId = cart.PaymentIntentId
            };

            _context.Orders.Add(order);
            // From this momment, we have saved the orders into our database, so we can remove the current basket
            _context.Carts.Remove(cart);

            if (orderDTO.SaveAddress)
            {
                var user = await _context.Users
                    .Include(a => a.Address)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);


                var newAddress = new UserAddress
                {
                    Street = orderDTO.ShippingAddress.Street,
                    Number = orderDTO.ShippingAddress.Number,
                    Address2 = orderDTO.ShippingAddress.Address2,
                    City = orderDTO.ShippingAddress.City,
                    State = orderDTO.ShippingAddress.State,
                    Zip = orderDTO.ShippingAddress.Zip,
                    Country = orderDTO.ShippingAddress.Country
                };

                user.Address = newAddress;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);

            return BadRequest(new ProblemDetails { Title = "Problem creating order" });
        }
    }
 }

