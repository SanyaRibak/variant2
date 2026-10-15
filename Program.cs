using MoiMarket.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(options =>
    options.Conventions.AddPageRouteModelConvention("/Index", model =>
        model.Selectors.Add(new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel
            {
                Template = "Items/Category/{categoryName:category}", Order = -1
            }
        })));
builder.Services.Configure<RouteOptions>(options =>
    options.ConstraintMap.Add("category", typeof(CategoryConstraint)));
builder.Services.AddSingleton<StoreService>();
builder.Services.AddDataProtection();
var app = builder.Build();
app.UseExceptionHandler("/Error");
app.UseStaticFiles();
app.UseRouting();
app.Use(async (context, next) =>
{
    var protector = context.RequestServices.GetRequiredService<IDataProtectionProvider>()
        .CreateProtector("MoiMarket.Owner");
    string? owner = null;
    if (context.Request.Cookies.TryGetValue("Moi2Owner", out var cookie))
    {
        try { owner = protector.Unprotect(cookie); }
        catch (System.Security.Cryptography.CryptographicException) { }
    }
    if (!Guid.TryParse(owner, out _))
    {
        owner = Guid.NewGuid().ToString();
        context.Response.Cookies.Append("Moi2Owner", protector.Protect(owner), new CookieOptions
        {
            HttpOnly = true, SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps, IsEssential = true,
            MaxAge = TimeSpan.FromDays(365)
        });
    }
    context.Items["Moi2Owner"] = owner;
    await next();
});
app.MapRazorPages();
app.Run();
