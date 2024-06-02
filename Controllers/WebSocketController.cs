using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly WebSocketService _webSocketService;
    private static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public WebSocketController(WebSocketService webSocketService, ApplicationDbContext context)
    {
        _context = context;
        _webSocketService = webSocketService;
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
            Console.WriteLine("Received message from client: " + clientEmail);
            Console.WriteLine("Current sockets dictionary: ");
            foreach (var key in _sockets.Keys)
            {
                Console.WriteLine(key);
            }
            if (!_sockets.ContainsKey(clientEmail))
            {
                // If no existing connection, add the new one
                _sockets.TryAdd(clientEmail, webSocket);
            }
            else
            {
                // If existing connection, use the old one
                webSocket = _sockets[clientEmail];
            }

            await Receive(webSocket, async (receiveResult, receiveBuffer) =>
            {
                if (!receiveResult.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                    Console.WriteLine("Received message from client: " + message);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    MessageDTO messageDTO = JsonSerializer.Deserialize<MessageDTO>(message, options);
                    var commandResult = await _webSocketService.ExecuteCommand(messageDTO, clientEmail);
                    for(int i = 0; i < commandResult.Destinations.Count; i++)
                    {
                        Console.WriteLine($"Sending '{commandResult.Messages[i]}' to {commandResult.Destinations[i]}");
                        if (_sockets.ContainsKey(commandResult.Destinations[i]))
                        {
                            var destinationWebSocket = _sockets[commandResult.Destinations[i]];
                            var responseMessage = Encoding.UTF8.GetBytes(commandResult.Messages[i]);
                            await destinationWebSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);
                        }
                        else
                        {
                            // Handle the case where the destination is not found in the _sockets dictionary
                            Console.WriteLine($"Destination {commandResult.Destinations[i]} not found in sockets dictionary.");
                        }
                    }
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
    
    private async Task Receive(WebSocket socket, Func<WebSocketReceiveResult, byte[], Task> handleMessage)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

            await handleMessage(result, buffer);
        }
    }
}