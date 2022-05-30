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
    public class VariantOptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public VariantOptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/VariantOption
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VariantOption>>> GetVariantOption([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.VariantOption.Include(p => p.ProductVariant).ThenInclude(p => p.Product).Where(p => p.ProductVariant.Product.CompanyId == companyId).ToListAsync();
        }

        // GET: api/VariantOption/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VariantOption>> GetVariantOptionById(string id)
        {
            var variantOption = await _context.VariantOption.Include(v => v.ProductVariant).FirstOrDefaultAsync(p => p.Id == id);
            var product = await _context.Product.FindAsync(variantOption.ProductVariant.ProductId);
            

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (variantOption == null)
            {
                return NotFound();
            }

            return variantOption;
        }

        // PUT: api/VariantOption/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariantOption(string id, VariantOption variantOption)
        {
            var productVariant = await _context.ProductVariant.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == variantOption.ProductVariantId);
            
            if(await _verification.UserIsCompanyMember(productVariant.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != variantOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(variantOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariantOptionExists(id))
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

        // POST: api/VariantOption
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VariantOption>> PostVariantOption(VariantOption variantOption)
        {
            var productVariant = await _context.ProductVariant.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == variantOption.ProductVariantId);
            

            if(await _verification.UserIsCompanyMember(productVariant.Product.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.VariantOption.Add(variantOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVariantOption", new { id = variantOption.Id }, variantOption);
        }

        // DELETE: api/VariantOption/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariantOption(string id)
        {
            
            var variantOption = await _context.VariantOption.FindAsync(id);
            var productVariant = await _context.ProductVariant.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == variantOption.ProductVariantId);

            if(await _verification.UserIsCompanyMember(productVariant.Product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (variantOption == null)
            {
                return NotFound();
            }

            _context.VariantOption.Remove(variantOption);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VariantOptionExists(string id)
        {
            return _context.VariantOption.Any(e => e.Id == id);
        }
    }
}
