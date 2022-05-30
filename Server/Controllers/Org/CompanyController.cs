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
using Application.Shared.Models.Enums;
using Application.Server.Services;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Verification _verification;

        public CompanyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _verification = verification;
        }



        // GET: api/Company
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {
            var members = await _context.Member.Where(m => m.ApplicationUserId == _userManager.GetUserId(User)).ToListAsync();
            var companyIds = members.Select(m => m.CompanyId).ToArray();

            return await _context.Company.Where(c => companyIds.Contains(c.Id)).ToListAsync();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<Company>> GetCompany([FromQuery] bool isDefault, [FromQuery] string slug)
        {
            Company company = null;

            if(isDefault)
            {
                company = await _context.Company.FirstOrDefaultAsync(c => c.IsDefault == isDefault);
            }

            else// if(!String.IsNullOrEmpty(slug)) 
            {
                company = await _context.Company.FirstOrDefaultAsync(c => c.Slug == slug);
            }

            if(company is null) {
                return NotFound();
            }
            else {
                return company;
            }
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(string id)
        {
            if(await _verification.UserIsCompanyMember(id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            var company = await _context.Company.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }
        

        // PUT: api/Company/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(string id, Company company)
        {
            if(await _verification.UserIsCompanyMember(company.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
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

        // POST: api/Company
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {

            if(CompanySlugExists(company.Slug))
            {
                return StatusCode(StatusCodes.Status409Conflict, "Slug already exists");
            }

            var user = await _userManager.GetUserAsync(User);

            var apiKeyController = new ApiKeyController(_context, _userManager, _verification);          

            company.Slug = company.Slug.ToLower();
            if(!UserHasDefaultCompany(user.Id)) {
                company.IsDefault = true;
            }

            _context.Company.Add(company);
            await _context.SaveChangesAsync();

            var member = new Member() {
                ApplicationUserId = user.Id,
                CompanyId = company.Id,
            };

            _context.Member.Add(member);
            await _context.SaveChangesAsync();

            var apiKey = new ApiKey() {
                ApplicationUserId = user.Id,
                CompanyId = company.Id,
            };
            await apiKeyController.PostApiKey(apiKey, company.Id, user.Id);

            // await CreateRolesAsync(user, company);

            var roles = new string[] {"Owner", "Admin", "Application", "User"};

            foreach(var role in roles) {
                await CreateRolesAsync(user, company, role);
            }
            

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        
        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            var company = await _context.Company.FindAsync(id);

            if(await _verification.UserIsCompanyMember(id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (company == null)
            {
                return NotFound();
            }

            _context.Company.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(string id)
        {
            return _context.Company.Any(e => e.Id == id);
        }

        private bool CompanySlugExists(string slug)
        {
            return _context.Company.Any(e => e.Slug == slug);
        }

        private bool UserHasDefaultCompany(string userId) {
            return _context.Member.Include(m => m.Company).Any(m => m.ApplicationUserId == userId && m.Company.IsDefault == true);
        }


        private async Task CreateRolesAsync(ApplicationUser user, Company company, string roleName)
        {    
            var ownerRoleExists = await _roleManager.RoleExistsAsync($"{company.Slug.ToUpper()}-{roleName}");
            if(!ownerRoleExists) {
                var role = new IdentityRole() {
                    Name = $"{company.Slug}-{roleName}",
                    NormalizedName = $"{company.Slug.ToUpper()}-{roleName}",
                };

                await _roleManager.CreateAsync(role);
            }
            await _userManager.AddToRoleAsync(user, $"{company.Slug}-{roleName}");

        }
    }
}
