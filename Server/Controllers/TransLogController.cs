#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Application.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Application.Server.Services;

namespace Application.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Verification _verification;

        public TransLogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, Verification verification)
        {
            _context = context;
            _userManager = userManager;
            _verification = verification;
        }

        // GET: api/TransLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransLog>>> GetTransLog([FromQuery] string companyId)
        {
            if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            return await _context.TransLog.Where(t => t.CompanyId == companyId).ToListAsync();
        }

        // GET: api/TransLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransLog>> GetTransLogById(string id)
        {
            var transLog = await _context.TransLog.FindAsync(id);

            if(await _verification.UserIsCompanyMember(transLog.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }
            
            if (transLog == null)
            {
                return NotFound();
            }

            return transLog;
        }

        // PUT: api/TransLog/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransLog(string id, TransLog transLog)
        {
            if(await _verification.UserIsCompanyMember(transLog.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (id != transLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(transLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransLogExists(id))
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

        // POST: api/TransLog
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransLog>> PostTransLog(TransLog transLog)
        {
            if(await _verification.UserIsCompanyMember(transLog.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            _context.TransLog.Add(transLog);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TransLogExists(transLog.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTransLog", new { id = transLog.Id }, transLog);
        }

        // DELETE: api/TransLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransLog(string id)
        {
            var transLog = await _context.TransLog.FindAsync(id);
            
            if(await _verification.UserIsCompanyMember(transLog.CompanyId, _userManager.GetUserId(User)) == false)
            {
                return Unauthorized();
            }

            if (transLog == null)
            {
                return NotFound();
            }

            _context.TransLog.Remove(transLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransLogExists(string id)
        {
            return _context.TransLog.Any(e => e.Id == id);
        }
    }
}
