using GA.Data;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var services = builder.Services;
var configuration = builder.Configuration;

services
     .AddAuthentication(o =>
     {
         // This forces challenge results to be handled by Google OpenID Handler, so there's no
         // need to add an AccountController that emits challenges for Login.
         o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
         // This forces forbid results to be handled by Google OpenID Handler, which checks if
         // extra scopes are required and does automatic incremental auth.
         o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
         // Default scheme that will handle everything else.
         // Once a user is authenticated, the OAuth2 token info is stored in cookies.
         o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
     })
     .AddCookie()
     .AddGoogleOpenIdConnect(options =>
     {
         //paste your client id and client secret
         options.ClientId = "";
         options.ClientSecret = "";
         options.Scope.Add("openid");
         options.Scope.Add("profile");
         options.Scope.Add("email");

         // additional scope(s)
         options.Scope.Add("https://www.googleapis.com/auth/analytics.readonly");
         options.SaveTokens = true;
         options.Events.OnTicketReceived = ctx =>
         {
             List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

             //print token list
             Console.WriteLine(tokens);
             foreach(var token in tokens)
             {
                 Console.WriteLine("Name :"+token.Name+" Value:"+token.Value);
                 Console.WriteLine(" ");
             }

             return Task.CompletedTask;
         };
     });



var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
