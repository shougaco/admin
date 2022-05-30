#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Application.Shared.Models.Org;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models;
using Application.Server.Services;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public StoreController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/Store
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStore([FromQuery] string companyId)
        {

            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.Store.Where(s => s.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStoreById(string id)
        {
            var store = await _context.Store.FindAsync(id);

            if(await _verification.UserIsCompanyMember(store.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // PUT: api/Store/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(string id, Store store)
        {

            if(await _verification.UserIsCompanyMember(store.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != store.Id)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/Store
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Store>> PostStore(Store store)
        {
            if(await _verification.UserIsCompanyMember(store.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.Store.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(string id)
        {
            var store = await _context.Store.FindAsync(id);

            if(await _verification.UserIsCompanyMember(store.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (store == null)
            {
                return NotFound();
            }

            _context.Store.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoreExists(string id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
