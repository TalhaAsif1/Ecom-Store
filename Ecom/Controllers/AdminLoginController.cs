using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;


/// <summary>
/// For admin registration and authorization
/// </summary>


[ApiController]
[Route("api/[controller]")]
public class AdminLoginController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public AdminLoginController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationModel model)
    {
        var newAdmin = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(newAdmin, model.Password);

        if (result.Succeeded)
        {
            var adminRoleExists = await roleManager.RoleExistsAsync("Admin");

            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await userManager.AddClaimAsync(newAdmin, new Claim(ClaimTypes.Role, "Admin"));
            await userManager.AddToRoleAsync(newAdmin, "Admin");

            return Ok(new { Message = "Admin registered successfully with role 'Admin'." });
        }

        return BadRequest(new { Message = "Admin registration failed.", Errors = result.Errors });
    }

}

public class AdminRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
