using System.Net.Http.Json;

namespace OurSpot.Services;

public class NotificacionesApiService
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public NotificacionesApiService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }


    public async Task NotificarCambioEstado(
        int eventoId,
        string nombreEvento,
        string estadoAnterior,
        string estadoNuevo)
    {
        if (estadoAnterior == estadoNuevo)
            return;

        var notificacion = new
        {
            EventoId = eventoId,
            NombreEvento = nombreEvento,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo
        };

        var clave = configuration["Notificaciones:ApiKey"];

        if (string.IsNullOrWhiteSpace(clave))
        {
            throw new InvalidOperationException(
                "No se configuró Notificaciones:ApiKey en OurSpot.");
        }

        using var solicitud = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7001/api/Notificaciones");

        solicitud.Headers.Add("X-OurSpot-Key", clave);
        solicitud.Content = JsonContent.Create(notificacion);

        using var respuesta = await httpClient.SendAsync(solicitud);

        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (!respuesta.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"La API rechazó la notificación. " +
                $"Código: {(int)respuesta.StatusCode}. " +
                $"Respuesta: {contenido}");
        }

        Console.WriteLine(
            $"NOTIFICACIÓN ENVIADA: {nombreEvento}, " +
            $"{estadoAnterior} -> {estadoNuevo}");
    }
}
