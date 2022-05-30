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

namespace Application.Server.Controllers.CollectionManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public CollectionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/Collection
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collection>>> GetCollection([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.Collection.Where(p => p.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Collection/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> GetCollectionById(string id)
        {
            var collection = await _context.Collection.FindAsync(id);

            if(await _verification.UserIsCompanyMember(collection.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (collection == null)
            {
                return NotFound();
            }

            return collection;
        }

        // PUT: api/Collection/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection(string id, Collection collection)
        {
            if(await _verification.UserIsCompanyMember(collection.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != collection.Id)
            {
                return BadRequest();
            }

            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/Collection
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Collection>> PostCollection(Collection collection)
        {
            if(await _verification.UserIsCompanyMember(collection.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.Collection.Add(collection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCollection", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collection/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(string id)
        {
            var collection = await _context.Collection.FindAsync(id);

            if(await _verification.UserIsCompanyMember(collection.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (collection == null)
            {
                return NotFound();
            }

            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectionExists(string id)
        {
            return _context.Collection.Any(e => e.Id == id);
        }
    }
}
