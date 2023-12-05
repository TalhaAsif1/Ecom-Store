using Ecom.Data;
using Ecom.Interfaces;
using Ecom.Repositoy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Ecom.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();

/*builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Regular", policy =>
        policy.RequireClaim("UserType", "Regular"));

    options.AddPolicy("Premium", policy =>
        policy.RequireClaim("UserType", "Premium"));

});*/
builder.Services.AddAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
   // Add information about your claims
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "oauth2"
            }
        },
        new string[] { "Admin", "User" }  // Add your roles here        
    }});
});

builder.Services.Configure<IdentityOptions>(options =>
{
    //// Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    // options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    //lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    //user settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddScoped<IProductInterface, ProductRepositroy>();
builder.Services.AddScoped<IOrderInterface, OrderRepository>();
builder.Services.AddScoped<IUserRegisterInterface, UserRegisterRepository>();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string email = "admin@admin.com";
    string password = "String!123";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser { UserName = email, Email = email };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),  // Example  claim
            new Claim(ClaimTypes.Email, email),   // Example email claim
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");

        // Add claims to the user
        await userManager.AddClaimsAsync(user, claims);
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
