using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgileRap_Process2.Data;
using AgileRap_Process2;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AgileRap_Process2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AgileRap_Process2Context") ?? throw new InvalidOperationException("Connection string 'AgileRap_Process2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
//builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Logins}/{action=Login}/{id?}");

app.Run();
