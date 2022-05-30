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
    public class BarcodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public BarcodeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/Barcode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barcode>>> GetBarcode([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.Barcode.Include(b => b.Product).Where(b => b.Product.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Barcode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Barcode>> GetBarcodeById(string id)
        {
            var barcode = await _context.Barcode.FindAsync(id);

            var product = await _context.Product.FindAsync(barcode.ProductId);

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (barcode == null)
            {
                return NotFound();
            }

            return barcode;
        }

        // PUT: api/Barcode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBarcode(string id, Barcode barcode)
        {
            var product = await _context.Product.FindAsync(barcode.ProductId);

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != barcode.Id)
            {
                return BadRequest();
            }

            _context.Entry(barcode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BarcodeExists(id))
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

        // POST: api/Barcode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Barcode>> PostBarcode(Barcode barcode)
        {
            var product = await _context.Product.FindAsync(barcode.ProductId);

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.Barcode.Add(barcode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBarcode", new { id = barcode.Id }, barcode);
        }

        // DELETE: api/Barcode/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarcode(string id)
        {
            var barcode = await _context.Barcode.FindAsync(id);

            var product = await _context.Product.FindAsync(barcode.ProductId);

            if(await _verification.UserIsCompanyMember(product.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }
            
            if (barcode == null)
            {
                return NotFound();
            }

            _context.Barcode.Remove(barcode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BarcodeExists(string id)
        {
            return _context.Barcode.Any(e => e.Id == id);
        }
    }
}
