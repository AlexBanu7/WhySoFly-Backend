using System.Collections.Concurrent;
using System.Threading.Tasks;
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
    private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public WebSocketController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var clientEmail = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (_sockets.ContainsKey(clientEmail))
            {
                // Close the old connection and accept the new one
                WebSocket oldSocket;
                if (_sockets.TryRemove(clientEmail, out oldSocket))
                {
                    await oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing old connection", CancellationToken.None);
                }
            }

            _sockets.TryAdd(clientEmail, webSocket);

            await Receive(webSocket, async (receiveResult, receiveBuffer) =>
            {
                if (!receiveResult.CloseStatus.HasValue)
                {
                    var market = await _context.Markets.FirstOrDefaultAsync();
                    var marketDto = MarketDisplayDTO.ToDTO(market);
                    var marketJson = JsonSerializer.Serialize(marketDto);
                    var helloWorld = Encoding.UTF8.GetBytes(marketJson);
                    await webSocket.SendAsync(new ArraySegment<byte>(helloWorld, 0, helloWorld.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    WebSocket socket;
                    _sockets.TryRemove(clientEmail, out socket);

                    await socket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                }
            });
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

            handleMessage(result, buffer);
        }
    }
}