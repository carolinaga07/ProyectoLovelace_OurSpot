using OurSpot.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Controladores de la API
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("OurSpotPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:7002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("OurSpotPolicy");

app.UseAuthorization();

// Controladores
app.MapControllers();

// Hub de notificaciones
app.MapHub<NotificacionesHub>("/notificacionesHub");

app.Run();
