using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.BusinessLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IRepository<Project>, ProjectRepo>();
builder.Services.AddScoped<IRepository<Ticket>, TicketRepo>();
builder.Services.AddScoped<IRepository<TicketWatcher>, TicketWatcherRepo>();
builder.Services.AddScoped<IRepository<Comment>, CommentRepo>();
builder.Services.AddScoped<IRepository<UserProject>, UserProjectRepo>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<UserManagerBusinessLogic>();
builder.Services.AddScoped<ProjectBusinessLogic>();
builder.Services.AddScoped<TicketBusinessLogic>();

//builder.Services.AddScoped<IUserRepo<ApplicationUser>, UserRepo>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<UserManagerBusinessLogic>();
builder.Services.AddScoped<ProjectBusinessLogic>();
builder.Services.AddScoped<AdminBusinessLogic>();
builder.Services.AddScoped<IUserRepo<ApplicationUser>, UserRepo>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Projects}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
