using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Server.Services.AddressService;
using TeamProject.Server.Services.AuthService;
using TeamProject.Server.Services.CategoryService;
using TeamProject.Server.Services.ProductTypeService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//DbConntection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//DI for services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
