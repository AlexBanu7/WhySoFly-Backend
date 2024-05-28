using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class WebSocketController : ControllerBase
{
    
    private readonly ApplicationDbContext _context;

    public WebSocketController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[1024 * 4];
            // Get the first Market entity, make it a dto and send it
            var market = await _context.Markets.FirstOrDefaultAsync();
            var marketDto = MarketDisplayDTO.ToDTO(market);
            var marketJson = JsonSerializer.Serialize(marketDto);
            var helloWorld = Encoding.UTF8.GetBytes(marketJson);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(helloWorld, 0, helloWorld.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}