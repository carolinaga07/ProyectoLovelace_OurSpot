using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components;
using OurSpot.Components.Pages.AdministradorPages.EventosPages.Services;
using OurSpot.Context;
using OurSpot.Security;
using OurSpot.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var ConStr = builder.Configuration.GetConnectionString("SqlConStr");

//builder.Services.AddScoped<EventosService>();

builder.Services.AddDbContextFactory<Contexto>(o => o.UseSqlServer(ConStr));

// controlador de la contrasenia con identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<Contexto>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/cuenta/login";
    options.AccessDeniedPath = "/acceso-denegado";
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

//builder.Services.AddDbContextFactory<Contexto>(o => o.UseInMemoryDatabase("OurSpotTempDb"));
builder.Services.AddScoped<EventosService>();
builder.Services.AddScoped<ContratacionesService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<LugaresService>();
builder.Services.AddScoped<PersonalService>();
builder.Services.AddScoped<RegistroClientesService>();
builder.Services.AddScoped<AdministradoresService>();
builder.Services.AddScoped<RegistroPersonalService>();
builder.Services.AddScoped<ClientesService>();
builder.Services.AddScoped<ContrasenasService>();
builder.Services.AddScoped<EventoImagenesService>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<CalificacionesService>();
builder.Services.AddBlazorBootstrap();

builder.Services.AddHttpClient<NotificacionesApiService>();
builder.Services.AddHostedService<EventosBackgroundService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// aqui se crean los roles 
await IdentitySeeder.CrearRolesAsync(app.Services);

var correoAdmin = app.Configuration["AdminSeed:Email"];
var claveAdmin = app.Configuration["AdminSeed:Password"];

if(app.Environment.IsDevelopment() 
    && !string.IsNullOrWhiteSpace(correoAdmin)
    && !string.IsNullOrWhiteSpace(claveAdmin))
{
    await IdentitySeeder.CrearAdministradorAsync(app.Services, correoAdmin, claveAdmin);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapGet("/api/eventos/imagenes/{imagenId:int}",
    async (
        int imagenId,
        HttpContext httpContext,
        EventoImagenesService imagenesService) =>
    {
        var usuario = httpContext.User;

        if (usuario.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var usuarioId = usuario.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )?.Value;

        if (string.IsNullOrEmpty(usuarioId))
            return Results.Unauthorized();

        var imagen = await imagenesService
            .ObtenerArchivoImagenAutorizada(
                imagenId,
                usuarioId,
                usuario.IsInRole("Administrador"));

        if (imagen == null)
            return Results.NotFound();

        return Results.File(
            imagen.Value.Contenido,
            imagen.Value.Tipo);
    })
    .RequireAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
