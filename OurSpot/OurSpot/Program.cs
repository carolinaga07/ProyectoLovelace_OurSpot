using Microsoft.EntityFrameworkCore;
using OurSpot.Components;
using OurSpot.Components.Pages.AdministradorPages.EventosPages.Services;
using OurSpot.Context;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/*var ConStr = builder.Configuration.GetConnectionString("SqlConStr");
builder.Services.AddDbContextFactory<Contexto>(o => o.UseSqlServer(ConStr)); */

//builder.Services.AddScoped<EventosService>();

builder.Services.AddDbContextFactory<Contexto>(o => o.UseInMemoryDatabase("OurSpotTempDb"));
builder.Services.AddScoped<EventosService>();

builder.Services.AddBlazorBootstrap();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
