using Microsoft.EntityFrameworkCore;
using Shortly.Data;
using Shortly.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUrlsService, UrlsService>();


var app = builder.Build();

app.UseHttpsRedirection();

//Add the redirect logic

app.Run();
