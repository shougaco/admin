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
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Application.Shared.Models;
using Application.Server.Services;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public ApiKeyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/ApiKey
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiKey>>> GetApiKey([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.ApiKey.Where(a => a.CompanyId == companyId).ToListAsync();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiKey>> GetApiKeyById([FromQuery] string companyId)
        {

            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            var apiKey = await _context.ApiKey.FirstOrDefaultAsync(a => a.CompanyId == companyId);

            return apiKey;
        }
        

        // PUT: api/ApiKey/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApiKey(string id, ApiKey apiKey)
        {
            if(await _verification.UserIsCompanyMember(apiKey.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != apiKey.Id)
            {
                return BadRequest();
            }

            _context.Entry(apiKey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiKeyExists(id))
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

        // POST: api/ApiKey
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiKey>> PostApiKey(ApiKey apiKey, [FromQuery] string companyId, string? userId  )
        {
            ApplicationUser user = !String.IsNullOrEmpty(userId) ? await _context.Users.FindAsync(userId) : await _userManager.GetUserAsync(User);

            if(await _verification.UserIsCompanyMember(companyId, user.Id) == false)
            {
                return Unauthorized();
            }

            apiKey.ApplicationUserId = user.Id;

            var company = await _context.Company.FindAsync(companyId);
            var key = GenerateKey(company.Slug);

            apiKey.Key = key;
            apiKey.CompanyId = companyId;
            _context.ApiKey.Add(apiKey);
            await _context.SaveChangesAsync();
            
            
            return CreatedAtAction("GetApiKey", new { id = apiKey.Id }, apiKey);
        }

        // DELETE: api/ApiKey/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiKey(string id)
        {
            var apiKey = await _context.ApiKey.FindAsync(id);

            if(await _verification.UserIsCompanyMember(apiKey.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (apiKey == null)
            {
                return NotFound();
            }

            _context.ApiKey.Remove(apiKey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiKeyExists(string id)
        {
            return _context.ApiKey.Any(e => e.Id == id);
        }

        private string GenerateKey(string companyName)
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            
            var prefix = companyName.ToUpper().Replace(" ", "")[^2];

            return prefix + Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .Substring(0, 33);
        }
    }
}
