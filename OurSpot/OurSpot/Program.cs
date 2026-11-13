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


app.MapGet("/api/cliente/eventos/imagenes/{imagenId:int}",
    async (
        int imagenId,
        System.Security.Claims.ClaimsPrincipal usuario,
        IDbContextFactory<Contexto> factory,
        IWebHostEnvironment entorno) =>
    {
        if (usuario.Identity?.IsAuthenticated != true ||
            !usuario.IsInRole("Cliente"))
            return Results.Forbid();

        var usuarioId = usuario.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioId))
            return Results.Forbid();

        await using var contexto =
            await factory.CreateDbContextAsync();

        var imagen = await contexto.EventoImagenes
            .AsNoTracking()
            .Where(i =>
                i.EventoImagenId == imagenId &&
                !i.Evento.Eliminado &&
                i.Evento.Clientes != null &&
                i.Evento.Clientes.ApplicationUserId == usuarioId)
            .Select(i => i.RutaImagen)
            .FirstOrDefaultAsync();

        if (imagen == null)
            return Results.NotFound();

        var nombre = Path.GetFileName(imagen);

        var ruta = Path.Combine(
            entorno.ContentRootPath,
            "ArchivosPrivados",
            "Eventos",
            nombre);

        if (!File.Exists(ruta))
            return Results.NotFound();

        var tipo = Path.GetExtension(nombre).ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return Results.File(
            ruta,
            contentType: tipo,
            enableRangeProcessing: false);
    })
    .RequireAuthorization();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
