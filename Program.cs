using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<cvContext>(optionsAction => optionsAction.UseSqlServer(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; 
        options.LogoutPath = "/logout"; 
        options.ExpireTimeSpan = TimeSpan.FromHours(1); 
        options.SlidingExpiration = true; 
    });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<cvContext>();

// add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
               //.AllowCredentials(); 
    });
});

var app = builder.Build();

// https request pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();