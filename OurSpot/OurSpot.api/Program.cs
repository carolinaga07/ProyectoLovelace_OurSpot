using OurSpot.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSignalR();

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

app.MapControllers();
app.MapHub<NotificacionesHub>("/notificacionesHub");

app.Run();
