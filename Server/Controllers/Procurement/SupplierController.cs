#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Application.Shared.Models.Procurement;
using Application.Server.Services;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models;

namespace Application.Server.Controllers.Procurement
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public SupplierController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/Supplier
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplier([FromQuery] string companyId)
        {

            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }
            
            return await _context.Supplier.Where(s => s.CompanyId == companyId).ToListAsync();
            
        }

        // GET: api/Supplier/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplierById(string id)
        {
            var supplier = await _context.Supplier.FindAsync(id);

            if(await _verification.UserIsCompanyMember(supplier.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        // PUT: api/Supplier/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(string id, Supplier supplier)
        {
            if(await _verification.UserIsCompanyMember(supplier.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != supplier.Id)
            {
                return BadRequest();
            }

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
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

        // POST: api/Supplier
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            if(await _verification.UserIsCompanyMember(supplier.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if(SupplierCodeExists(supplier.Code)) {
                return BadRequest("Supplier Code already exists");
            }
            
            _context.Supplier.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSupplier", new { id = supplier.Id }, supplier);
        }

        // DELETE: api/Supplier/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(string id)
        {
            var supplier = await _context.Supplier.FindAsync(id);

            if(await _verification.UserIsCompanyMember(supplier.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (supplier == null)
            {
                return NotFound();
            }

            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(string id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }

        private bool SupplierCodeExists(string code)
        {
            return _context.Supplier.Any(e => e.Code == code);
        }

        
    }
}
