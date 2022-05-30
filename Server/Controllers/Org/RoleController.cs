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
using Application.Server.Services;
using Application.Shared.Models;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Verification _verification;

        public RoleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/IdentityRole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRole([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }
            
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Id== companyId);

            return await _roleManager.Roles.Where(r => r.Name.StartsWith(company.Slug.ToUpper())).ToListAsync();
        }

        // GET: api/IdentityRole/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IdentityRole>> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var companySlug = role.Name.Split('_')[0].ToUpper();
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Slug == companySlug);

            if(await _verification.UserIsCompanyMember(company.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // PUT: api/IdentityRole/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(string id, IdentityRole role)
        {
            var companySlug = role.Name.Split('_')[0].ToUpper();
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Slug == companySlug);

            if(await _verification.UserIsCompanyMember(company.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoleExistsAsync(id))
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

        // POST: api/IdentityRole
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IdentityRole>> PostRole(IdentityRole role)
        {
            var companySlug = role.Name.Split('_')[0].ToUpper();
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Slug == companySlug);

            if(await _verification.UserIsCompanyMember(company.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            try
            {
                await _roleManager.CreateAsync(role);
            }
            catch (DbUpdateException)
            {
                if (await RoleExistsAsync(role.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }

        // DELETE: api/IdentityRole/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var companySlug = role.Name.Split('_')[0].ToUpper();
            var company = await _context.Company.FirstOrDefaultAsync(c => c.Slug == companySlug);

            if(await _verification.UserIsCompanyMember(company.Id, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (role == null)
            {
                return NotFound();
            }


            await _roleManager.DeleteAsync(role);

            return NoContent();
        }

        private async Task<bool> RoleExistsAsync(string id)
        {
            return await _roleManager.RoleExistsAsync(id);
        }
    }
}
