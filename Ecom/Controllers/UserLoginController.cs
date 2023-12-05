using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;


/// <summary>
/// for user registration and authorization
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class UserLoginController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public UserLoginController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
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
            Email = model.Email,
        };

        var result = await userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            var userRoleExists = await roleManager.RoleExistsAsync("User");

            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            await userManager.AddToRoleAsync(newUser, "User");
            await userManager.AddClaimAsync(newUser, new Claim(ClaimTypes.Role, "User"));

            // Set user type claim based on the selected type
            var userTypeClaim = new Claim("UserType", model.UserType);
            await userManager.AddClaimAsync(newUser, userTypeClaim);

            return Ok(new { Message = $"User registered successfully with role 'User'" });
        }

        return BadRequest(new { Message = "User registration failed.", Errors = result.Errors });
    }

}

public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set;}
    public string UserType { get; set; } // Add UserType field

}
