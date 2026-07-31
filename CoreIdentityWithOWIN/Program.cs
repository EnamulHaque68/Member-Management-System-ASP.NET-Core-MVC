using CoreIdentityWithOWIN.DTOS;
using CoreIdentityWithOWIN.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(op =>
    op.UseSqlServer(builder.Configuration.GetConnectionString("con")));
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(op =>
{
    op.Password.RequiredLength = 5;
    op.Password.RequireNonAlphanumeric = false;
    op.Password.RequireDigit = true;
    op.Password.RequireUppercase = false;
    op.Password.RequireLowercase = false;
    op.SignIn.RequireConfirmedAccount = false;
    op.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(op =>
{
    op.LoginPath = "/Account/Login";
    op.LogoutPath = "/Account/Logout";
    op.AccessDeniedPath = "/Account/AccessDenied";
    op.ExpireTimeSpan = TimeSpan.FromDays(7);
    op.SlidingExpiration = true;
    op.Cookie.HttpOnly = true;
    op.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("AuthenticatedUsers", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdminAsync(services);
}
app.MapControllerRoute(
    name: "account_login",
    pattern: "Account/Login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "account_register",
    pattern: "Account/Register",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "account_logout",
    pattern: "Account/Logout",
    defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "account",
    pattern: "Account/AccessDenied",
    defaults: new { controller = "Account", action = "AccessDenied" });

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "admin_listroles",
    pattern: "Admin/Roles",
    defaults: new { controller = "Admin", action = "ListRoles" });

app.MapControllerRoute(
    name: "admin_createrole",
    pattern: "Admin/Roles/Create",
    defaults: new { controller = "Admin", action = "CreateRole" });

app.MapControllerRoute(
    name: "admin_deleterole",
    pattern: "Admin/Roles/Delete/{id}",
    defaults: new { controller = "Admin", action = "DeleteRole" });

app.MapControllerRoute(
    name: "admin_manageuserroles",
    pattern: "Admin/Users/{userId}/Roles",
    defaults: new { controller = "Admin", action = "ManageUserRoles" });

app.MapControllerRoute(
    name: "members",
    pattern: "Members",
    defaults: new { controller = "Members", action = "Index" });
app.MapControllerRoute(
    name: "members_createpartial",
    pattern: "Members/CreatePartial",
    defaults: new { controller = "Members", action = "CreatePartial" });

app.MapControllerRoute(
    name: "Create",
    pattern: "Members/Create",
    defaults: new { controller = "Members", action = "Create" });

app.MapControllerRoute(
    name: "members_createmember",
    pattern: "Members/CreateMember",
    defaults: new { controller = "Members", action = "CreateMember" });

app.MapControllerRoute(
    name: "members_deletemember",
    pattern: "Members/DeleteMember/{id}",
    defaults: new { controller = "Members", action = "DeleteMember" });

app.MapControllerRoute(
    name: "members_editpartial",
    pattern: "Members/EditPartial/{id}",
    defaults: new { controller = "Members", action = "EditPartial" });

app.MapControllerRoute(
    name: "members_editmember",
    pattern: "Members/EditMember",
    defaults: new { controller = "Members", action = "EditMember" });

app.MapControllerRoute(
    name: "viewmember",
    pattern: "ViewMember",
    defaults: new { controller = "ViewMember", action = "Index" });


app.MapControllerRoute(
    name: "membertypes",
    pattern: "MemberTypes",
    defaults: new { controller = "MemberTypes", action = "Index" });

app.MapControllerRoute(
    name: "membertypes_details",
    pattern: "MemberTypes/Details/{id}",
    defaults: new { controller = "MemberTypes", action = "Details" });

app.MapControllerRoute(
    name: "membertypes_create",
    pattern: "MemberTypes/Create",
    defaults: new { controller = "MemberTypes", action = "Create" });

app.MapControllerRoute(
    name: "membertypes_edit",
    pattern: "MemberTypes/Edit/{id}",
    defaults: new { controller = "MemberTypes", action = "Edit" });

app.MapControllerRoute(
    name: "membertypes_delete",
    pattern: "MemberTypes/Delete/{id}",
    defaults: new { controller = "MemberTypes", action = "Delete" });
app.MapControllerRoute(
    name: "home",
    pattern: "Home",
    defaults: new { controller = "Home", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@library.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}