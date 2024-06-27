using DeckApi.ServiceInterface;
using DeckApi.ServiceInterface.Data;
using Microsoft.AspNetCore.Identity;
using ServiceStack.Logging;

var builder = WebApplication.CreateBuilder(args);
LogManager.LogFactory = new ConsoleLogFactory(debugEnabled:true);


builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorization();


builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


// Register ServiceStack APIs, Dependencies and Plugins:
builder.Services.AddServiceStack(typeof(CartService).Assembly);

var app = builder.Build();

app.UseAuthorization();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
// Register ServiceStack AppHost
app.UseServiceStack(new AppHost(), options => {
    options.MapEndpoints();
});

app.Run();