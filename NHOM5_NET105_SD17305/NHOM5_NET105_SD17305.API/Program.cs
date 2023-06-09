using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHOM5_NET105_SD17305.Data.Data;
using NHOM5_NET105_SD17305.Data.IServices;
using NHOM5_NET105_SD17305.Data.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FastFoodDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FastFood"), c => c.MigrationsAssembly("NHOM5_NET105_SD17305.API"));
});
// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<FastFoodDbContext>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();    
builder.Services.AddScoped<UserServices>();

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/SignIn/AccessDenied";
        options.Cookie.Name = "SignInCookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCookiePolicy();
app.MapControllers();

app.Run();
