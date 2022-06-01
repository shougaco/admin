using System.Text;
using Application.Server.Data;
using Application.Server.Services;
using Application.Shared.Models;
using Application.Shared.Models.Org;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Server.Controllers.Org;


// [Authorize]
[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IConfiguration _configuration;
    private readonly Verification _verification;

    public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, IConfiguration configuration, Verification verification)
    {
        _context = context;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _configuration = configuration;
        _verification = verification;
    }


    // Add new user
    [HttpPost]
    public async Task<ActionResult<ApplicationUser>> AddUserAsync(UserInputModel userInputModel, [FromQuery] string companyId)
    {
        if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
        {
            return Unauthorized();
        }
        
        if(!UsernameExists(userInputModel.UserName) && !EmailExists(userInputModel.Email)) {

            var user = CreateUser();

            user.EmailConfirmed = true;

            await _userStore.SetUserNameAsync(user, userInputModel.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, userInputModel.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, userInputModel.Password);

            if(result.Succeeded) {

                if(!String.IsNullOrEmpty(companyId)){
                    // add member
                    var member = new Member() {
                        ApplicationUserId = user.Id,
                        CompanyId = companyId
                    };

                    _context.Member.Add(member);
                    await _context.SaveChangesAsync();
                }

                // await _userManager.AddToRolesAsync(user, userInputModel.RoleNames);
                foreach(var role in userInputModel.RoleNames) {
                    await _userManager.AddToRoleAsync(user, role);
                }

                var apiKeyController = new ApiKeyController(_context, _userManager, _verification);    
                var apiKey = new ApiKey() {
                    ApplicationUserId = user.Id,
                    CompanyId = companyId
                };

                await apiKeyController.PostApiKey(apiKey, companyId, user.Id);
            }

            //return CreatedAtAction("GetUserByIdAsync", new { id = userId }, user); 
            return user;
        }
        else if(UsernameExists(userInputModel.UserName)){
            return StatusCode(406, $"The username already exists.");  
        }
        else {
            return StatusCode(406, $"The email already exists.");  
        }

    }

    [HttpPut("password/reset/{id}")]
    public async Task<ActionResult<ApplicationUser>> ResetPasswordAsync(string id, UserInputModel userInput, string companyId)
    {
        var user = await _userManager.FindByNameAsync(userInput.UserName);

        if(await _verification.UserIsCompanyMember(companyId, _userManager.GetUserId(User)) == false)
        {
            return Unauthorized();
        }

        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var newCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var addPasswordResult = await _userManager.ResetPasswordAsync(user, newCode ,userInput.Password);

        if(addPasswordResult.Succeeded) {
            Console.WriteLine("Password reset successful.");
        }
        else {
            Console.WriteLine("Password reset failed.");
            foreach (var error in addPasswordResult.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
        }
        
        return user;

    }

    private bool UserExists(string id)
    {
        return _context.ApplicationUser.Any(e => e.Id == id);
    }

    private bool UsernameExists(string username)
    {
        return _context.ApplicationUser.Any(e => e.UserName == username);
    }

    private bool EmailExists(string email)
    {
        return _context.ApplicationUser.Any(e => e.Email == email);
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
    
}