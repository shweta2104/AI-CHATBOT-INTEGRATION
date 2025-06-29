using Microsoft.EntityFrameworkCore;
using econest.Data;
using econest.Service;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSingleton<OpenAIService>();
//var builder = WebApplication.CreateBuilder(args);

// Register your Hugging Face service
builder.Services.AddSingleton<HuggingFaceService>();

builder.Services.AddControllersWithViews();

// Add services to the container
builder.Services.AddHttpContextAccessor(); // Enables access to HttpContext
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // 🔴 Must come before Authorization and after UseRouting

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
