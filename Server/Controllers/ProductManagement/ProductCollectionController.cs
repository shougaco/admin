#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models;
using Application.Server.Services;
using Application.Shared.Models.ProductManagement;

namespace Application.Server.Controllers.ProductCollectionManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCollectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public ProductCollectionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/ProductCollection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCollection>>> GetProductCollection([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.ProductCollection.Include(p => p.Product).Where(p => p.Product.CompanyId == companyId).ToListAsync();
        }

        // GET: api/ProductCollection/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCollection>> GetProductCollectionById(string id)
        {
            var productCollection = await _context.ProductCollection.Include(p => p.Product).FirstAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productCollection.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productCollection == null)
            {
                return NotFound();
            }

            return productCollection;
        }

        // PUT: api/ProductCollection/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCollection(string id, ProductCollection productCollection)
        {
            var product = await _context.Product.FindAsync(productCollection.ProductId);
            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != productCollection.Id)
            {
                return BadRequest();
            }

            _context.Entry(productCollection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCollectionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductCollection
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductCollection>> PostProductCollection(ProductCollection productCollection)
        {
            var product = await _context.Product.FindAsync(productCollection.ProductId);
            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.ProductCollection.Add(productCollection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCollection", new { id = productCollection.Id }, productCollection);
        }

        // DELETE: api/ProductCollection/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCollection(string id)
        {
            var productCollection = await _context.ProductCollection.Include(p => p.Product).FirstAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productCollection.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productCollection == null)
            {
                return NotFound();
            }

            _context.ProductCollection.Remove(productCollection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductCollectionExists(string id)
        {
            return _context.ProductCollection.Any(e => e.Id == id);
        }
    }
}
