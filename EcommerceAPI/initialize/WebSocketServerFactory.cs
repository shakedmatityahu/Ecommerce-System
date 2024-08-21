using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace EcommerceAPI.initialize
{
    public static class WebSocketServerFactory
    {
        public static WebSocketServer CreateWebSocketServer(Configurate configurate)
        {
            string port = configurate.Parse();
            WebSocketServer notificationServer = new WebSocketServer($"ws://{GetLocalIPAddress()}:{port}");
            return notificationServer;
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }

}