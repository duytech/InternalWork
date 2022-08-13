using InternalWork.Auth.Common.Models;
using InternalWork.Auth.Common.SettingModels;
using InternalWork.Auth.Data.Entities;
using InternalWork.Auth.Data.Utils;
using InternalWork.Auth.Services.Services;
using InternalWork.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

builder.Configuration
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", false, true)
  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, true)
  .AddEnvironmentVariables();

// configure strongly typed settings object
builder.Services.Configure<AuthSetting>(builder.Configuration.GetSection("AuthSetting"));

// configure authentication by jwt token
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            //ValidAudience = builder.Configuration["Jwt:Audience"],
            //ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSetting:Secret"]))
        };
    });
builder.Services.AddAuthorization(option =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    option.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
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
builder.Services.AddScoped<IUserService, UserService>();

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
    var adminData = new SampleDataSetting();
    app.Configuration.GetSection("AdminData").Bind(adminData);
    if (adminData.Users != null && adminData.Users.Length > 0)
    {
        var adminDataAccount = adminData.Users.First();
        CreateUser(userManager, adminDataAccount);
    }

    var sampleData = new SampleDataSetting();
    app.Configuration.GetSection("SampleData").Bind(sampleData);
    if (sampleData.Users != null && sampleData.Users.Length > 0)
    {
        foreach(var sample in sampleData.Users)
        {
            CreateUser(userManager, sample);
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void CreateUser(UserManager<AppIdentityUser> userManager, UserSetting sample)
{
    var user = userManager.FindByEmailAsync(sample.Email).Result;
    if (user is null)
    {
        var identityUser = new AppIdentityUser
        {
            Email = sample.Email,
            UserName = sample.Email,
            EmailConfirmed = true
        };
        var result = userManager.CreateAsync(identityUser, sample.Password).Result;

        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(identityUser, AppRole.Member).Wait();
        }
    }
}