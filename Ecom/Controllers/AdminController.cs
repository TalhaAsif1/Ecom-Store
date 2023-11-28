using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationModel model)
    {
        // Add your validation logic for the registration model

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
