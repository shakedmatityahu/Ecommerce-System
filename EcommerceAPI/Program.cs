using System.Net;
using System.Net.Sockets;
using EcommerceAPI.Controllers;
using EcommerceAPI.initialize;
using MarketBackend.Domain.Payment;
using MarketBackend.Domain.Shipping;
using MarketBackend.Services;
using MarketBackend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using WebSocketSharp.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddSingleton<IShippingSystemFacade>(new RealShippingSystem("https://damp-lynna-wsep-1984852e.koyeb.app/"));
builder.Services.AddSingleton<IPaymentSystemFacade>(new RealPaymentSystem("https://damp-lynna-wsep-1984852e.koyeb.app/"));
builder.Services.AddSingleton<IClientService, ClientService>();
builder.Services.AddSingleton<IMarketService, MarketService>();
builder.Services.AddSingleton<Configurate>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173")
                           .AllowAnyHeader()
                           .AllowAnyMethod());
});

// Use a factory method to create Configurate with the necessary dependencies
builder.Services.AddSingleton<Configurate>(sp => 
{
    var marketService = sp.GetRequiredService<IMarketService>();
    var clientService = sp.GetRequiredService<IClientService>();
    return new Configurate(marketService, clientService);
});

// Register WebSocket servers using factory methods
builder.Services.AddSingleton<WebSocketServer>(sp =>
{
    var configurate = sp.GetRequiredService<Configurate>();
    string port = configurate.Parse();
    var alertServer = new WebSocketServer("ws://127.0.0.1:" + port);
    alertServer.Start();
    return alertServer;
});

builder.Services.AddSingleton<WebSocketServer>(sp =>
{
    // var configurate = sp.GetRequiredService<Configurate>();
    // string port = configurate.Parse();
    var logServer = new WebSocketServer($"ws://127.0.0.1:{4570}");
    logServer.Start();
    return logServer;
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();
app.MapControllers();

var configurate = app.Services.GetRequiredService<Configurate>();
configurate.Parse();
// string port = configurate.Parse();
// WebSocketServer alertServer = new WebSocketServer($"ws://{GetLocalIPAddress()}:{port}");
// WebSocketServer logServer = new WebSocketServer($"ws://{GetLocalIPAddress()}:{port + 1}");

// alertServer.Start();
// logServer.Start();

app.Run();

static string GetLocalIPAddress()
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
