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
using Microsoft.AspNetCore.Authorization;
using Application.Server.Services;

namespace Application.Server.Controllers.Org
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public MemberController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/Member
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMember()
        {
            return await _context.Member.ToListAsync();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Member>>> GetMember([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.Member.Include(m => m.ApplicationUser).Where(m => m.CompanyId == companyId).ToListAsync();
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMemberById(string id)
        {
            var member = await _context.Member.FindAsync(id);

            if(await _verification.UserIsCompanyMember(member.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Member/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(string id, Member member)
        {
            if(await _verification.UserIsCompanyMember(member.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != member.Id)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Member
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member, [FromQuery] string? username)
        {
            if(await _verification.UserIsCompanyMember(member.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if(!String.IsNullOrEmpty(username)) {
                var user = await _userManager.FindByNameAsync(username);
                member.ApplicationUserId = user.Id;
            }
            _context.Member.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var member = await _context.Member.FindAsync(id);

            if(await _verification.UserIsCompanyMember(member.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (member == null)
            {
                return NotFound();
            }

            _context.Member.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(string id)
        {
            return _context.Member.Any(e => e.Id == id);
        }
    }
}
