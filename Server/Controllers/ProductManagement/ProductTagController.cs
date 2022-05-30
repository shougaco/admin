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

namespace Application.Server.Controllers.ProductTagManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public ProductTagController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/ProductTag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductTag>>> GetProductTag([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.ProductTag.Include(p => p.Product).Where(p => p.Product.CompanyId == companyId).ToListAsync();
        }

        // GET: api/ProductTag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductTag>> GetProductTagById(string id)
        {
            var productTag = await _context.ProductTag.Include(p => p.Product).FirstAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productTag.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productTag == null)
            {
                return NotFound();
            }

            return productTag;
        }

        // PUT: api/ProductTag/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductTag(string id, ProductTag productTag)
        {
            var product = await _context.Product.FindAsync(productTag.ProductId);
            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != productTag.Id)
            {
                return BadRequest();
            }

            _context.Entry(productTag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTagExists(id))
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

        // POST: api/ProductTag
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductTag>> PostProductTag(ProductTag productTag)
        {
            var product = await _context.Product.FindAsync(productTag.ProductId);
            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.ProductTag.Add(productTag);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductTag", new { id = productTag.Id }, productTag);
        }

        // DELETE: api/ProductTag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductTag(string id)
        {
            var productTag = await _context.ProductTag.Include(p => p.Product).FirstAsync(p => p.Id == id);

            if(await _verification.UserIsCompanyMember(productTag.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (productTag == null)
            {
                return NotFound();
            }

            _context.ProductTag.Remove(productTag);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductTagExists(string id)
        {
            return _context.ProductTag.Any(e => e.Id == id);
        }
    }
}
