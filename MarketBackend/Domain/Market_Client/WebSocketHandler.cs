using MarketBackend.Domain.Market_Client;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebSocketHandler : WebSocketBehavior
{
    private readonly NotificationManager _notificationManager;
    private readonly string _username;

    public WebSocketHandler(NotificationManager notificationManager, string username)
    {
        _notificationManager = notificationManager;
        _username = username;
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        _notificationManager.AddSession(ID, Sessions);
        Console.WriteLine($"New WebSocket connection established for user {_username}. Session ID: {ID}");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        base.OnClose(e);
        _notificationManager.RemoveSession(ID);
        Console.WriteLine($"WebSocket connection closed for user {_username}. Session ID: {ID}");
    }
}
