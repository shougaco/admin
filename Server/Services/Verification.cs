using Application.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Server.Services;

public class Verification
{
    private readonly ApplicationDbContext _context;

    public Verification(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> UserIsCompanyMember(string companyId, string userId)
    {
        // TODO EDI: Verify company with user membership
        var member = await _context.Member.FirstOrDefaultAsync(m => m.ApplicationUserId == userId && m.CompanyId == companyId);

        if(member is not null) {
            return true;
        }
        else {
            return false;
        }
    }
}