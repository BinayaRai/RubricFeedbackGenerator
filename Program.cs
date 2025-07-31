using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AIS_RubricFeedbackGenerator.Data;
using AIS_RubricFeedbackGenerator.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AIS_RubricFeedbackGeneratorContextConnection") ?? throw new InvalidOperationException("Connection string 'AIS_RubricFeedbackGeneratorContextConnection' not found.");

builder.Services.AddDbContext<AIS_RubricFeedbackGeneratorContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));


builder.Services.AddDefaultIdentity<AIS_RubricFeedbackGeneratorUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AIS_RubricFeedbackGeneratorContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
