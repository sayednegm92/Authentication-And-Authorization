using AuthenticationApp;
using AuthenticationApp.Authentication;
using AuthenticationApp.Authorization;
using AuthenticationApp.Data;
using AuthenticationApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.Filters.Add<PermissionBasedAuthorizationFilter>();
 });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductService, ProductService>();
//Basic Authentication
//builder.Services.AddAuthentication()
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic",null);

//Bearer Authentication
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddAuthorization(options => {
    options.AddPolicy("SuperUsersOnly", builder =>
    {
        builder.RequireRole("Admin","SuperUser");

    });
    options.AddPolicy("AgeGreaterThan25", builder =>
    {
        builder.AddRequirements(new AgeGreaterThan25Requirement());
    });
    options.AddPolicy("AgeGreaterThan26", builder =>
    {
        builder.RequireAssertion(context =>
        {
            var dob = DateTime.Parse(context.User.FindFirstValue("DateOfBirth"));
            return DateTime.Today.Year-dob.Year >26;
        });
    });
    options.AddPolicy("EmployeesOnly", builder =>
    {
        //builder.RequireClaim("UserType");
        builder.RequireClaim("UserType", "Employee");
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, AgeAuthorizationHandler>();
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,option =>
    {
        option.SaveToken = true;//Save Token in Authentication Property
        //How To Validate Token
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer=jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience=jwtOptions.Audience,
            ValidateIssuerSigningKey=true,
            IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });
var app = builder.Build();

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


//Permission Based
//Roles Based
//Policy Based