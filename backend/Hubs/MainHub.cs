using Microsoft.AspNetCore.SignalR;

namespace DivHunt.Hubs;

public class MainHub : Hub
{

    public async Task ReceiveCoordinates(Coordinates coordinates) =>
        await Clients.All.SendAsync("SendCoordinates", coordinates);
}

public record Coordinates(float Horizontal, float Vertical);