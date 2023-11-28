using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public IdentityController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModel model)
    {
        var newUser = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            // Check if the role "User" exists, create it if not
            var userRoleExists = await roleManager.RoleExistsAsync("User");

            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Add the user to the "User" role during registration
            await userManager.AddToRoleAsync(newUser, "User");

            return Ok(new { Message = "User registered successfully with role 'User'." });
        }

        return BadRequest(new { Message = "User registration failed.", Errors = result.Errors });
    }

    // You can add other actions for managing users, roles, etc.
}

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    // Add other properties as needed
}
