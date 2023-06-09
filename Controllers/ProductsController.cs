using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.Extensions;
using StoreApi.Models;
using StoreApi.RequestHelpers;



namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly PrintStoreDbContext _context;
        public ProductsController(PrintStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery] ProductsParams productsParams)
        {
            var query = _context.Products
            .Sort(productsParams.OrderBy)
            .Search(productsParams.SearchTerm)
            .Filter(productsParams.Brands, productsParams.Types)
            .AsQueryable();

            var products = await PagedList<Product>.ToPagedList(query, productsParams.PageNumber, productsParams.PageSize);

            Response.AddPaginationHeader(products.MetaData);

            return products;

        }

        //route: api/product/3
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return Ok(new { brands, types });
        }
    }
}
