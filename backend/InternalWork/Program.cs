using InternalWork.Auth.Common.Models;
using InternalWork.Auth.Data.Entities;
using InternalWork.Auth.Data.Utils;
using InternalWork.Service;
using InternalWork.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure strongly typed settings object
builder.Services.Configure<AuthSetting>(builder.Configuration.GetSection("AuthSetting"));

// configure authentication by jwt token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            //ValidAudience = builder.Configuration["Jwt:Audience"],
            //ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSetting:Secret"]))
        };
    });

// configure mysql database context connection setting
var serverVersion = new MySqlServerVersion(new Version(8, 0, 20));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(builder.Configuration["DbSetting:ConnectionString"], serverVersion);
});

builder.Services.AddIdentity<AppIdentityUser, IdentityRole<Guid>>(options => 
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// configure DI for application services
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    // create user roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    string[] roleNames = AppRole.GetArray();

    foreach (string roleName in roleNames)
    {
        bool roleExist = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();

        if (!roleExist)
        {
            Console.WriteLine($"### creating role {roleName}");
            roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).GetAwaiter().GetResult();
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
    var admin = userManager.FindByEmailAsync("admin@company.com").GetAwaiter().GetResult();
    if (admin is null)
    {
        var adminIdentity = new AppIdentityUser
        {
            Email = "admin@company.com",
            UserName = "admin@company.com",
            EmailConfirmed = true
        };
        var result = userManager.CreateAsync(adminIdentity, "l6kxet@A").GetAwaiter().GetResult();

        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminIdentity, AppRole.Admin);
        }
    }

    roleManager.Dispose();
    userManager.Dispose();
    context.Dispose();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
