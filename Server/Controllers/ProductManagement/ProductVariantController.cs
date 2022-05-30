#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Application.Shared.Models.ProductManagement;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models;
using Application.Server.Services;

namespace Application.Server.Controllers.ProductManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public ProductVariantController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/ProductVariant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVariant>>> GetProductVariant([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.ProductVariant.Include(p => p.Product).Where(p => p.Product.CompanyId == companyId).ToListAsync();
        }

        // GET: api/ProductVariant/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariant>> GetProductVariantById(string id)
        {
            var productVariant = await _context.ProductVariant.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productVariant.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productVariant == null)
            {
                return NotFound();
            }

            return productVariant;
        }

        // PUT: api/ProductVariant/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductVariant(string id, ProductVariant productVariant)
        {
            var product = await _context.Product.FindAsync(productVariant.ProductId);

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != productVariant.Id)
            {
                return BadRequest();
            }

            _context.Entry(productVariant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductVariantExists(id))
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

        // POST: api/ProductVariant
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductVariant>> PostProductVariant(ProductVariant productVariant)
        {
            var product = await _context.Product.FindAsync(productVariant.ProductId);

            if(await _verification.UserIsCompanyMember(product.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.ProductVariant.Add(productVariant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductVariant", new { id = productVariant.Id }, productVariant);
        }

        // DELETE: api/ProductVariant/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariant(string id)
        {
            var productVariant = await _context.ProductVariant.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productVariant.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productVariant == null)
            {
                return NotFound();
            }

            _context.ProductVariant.Remove(productVariant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductVariantExists(string id)
        {
            return _context.ProductVariant.Any(e => e.Id == id);
        }
    }
}
