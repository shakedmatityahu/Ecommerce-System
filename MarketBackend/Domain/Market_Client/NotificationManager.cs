using System.Collections.Concurrent;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MarketBackend.Domain.Market_Client
{
    public class NotificationManager
    {
        private static NotificationManager _alertsManager = null;
        private static readonly object _lock = new object();
        private WebSocketServer _alertsServer;
        private readonly ConcurrentDictionary<string, WebSocketSessionManager> _sessions = new ConcurrentDictionary<string, WebSocketSessionManager>();

        private NotificationManager(WebSocketServer alertsServer)
        {
            _alertsServer = alertsServer;
        }

        private NotificationManager()
        {
        }

        public static NotificationManager GetInstance(WebSocketServer alertsServer)
        {
            if (_alertsManager != null)
            {
                _alertsManager._alertsServer = alertsServer;
                return _alertsManager;
            }
            lock (_lock)
            {
                _alertsManager ??= new NotificationManager(alertsServer);
            }
            return _alertsManager;
        }

        public static NotificationManager GetInstance()
        {
            if (_alertsManager != null)
                return _alertsManager;
            lock (_lock)
            {
                _alertsManager ??= new NotificationManager();
            }
            return _alertsManager;
        }

        public void AddSession(string sessionId, WebSocketSessionManager sessionManager)
        {
            _sessions[sessionId] = sessionManager;
        }

        public void RemoveSession(string sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        public void SendNotification(string message, string username)
        {
            var relativePath = $"/{username}-alerts";

            if (_alertsServer == null || !_alertsServer.WebSocketServices.TryGetServiceHost(relativePath, out var webSocketService))
                return;

            var json = JsonSerializer.Serialize(new { message });

            foreach (var session in webSocketService.Sessions.Sessions)
            {
                try
                {
                    var webSocket = session.Context.WebSocket;
                    if (webSocket?.IsAlive ?? false)
                    {
                        webSocket.Send(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message to session: {ex.Message}");
                }
            }
        }
    }
}
