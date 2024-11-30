using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Collections.Generic;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly IHubContext<LogHub> _hubContext;

    public RedisService(string connectionString, IHubContext<LogHub> hubContext)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
        _hubContext = hubContext;
    }

    public void AddLog(string logId, string message)
    {
        _db.HashSet("logs", logId, message);
        // Enviar o log para todos os clientes conectados via SignalR
        _hubContext.Clients.All.SendAsync("ReceiveLog", logId, message);
    }

    public IEnumerable<KeyValuePair<string, string>> GetLogs()
    {
        return _db.HashGetAll("logs").ToStringDictionary();
    }
}