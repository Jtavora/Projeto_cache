using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class LogHub : Hub
{
    private readonly RedisService _redisService;

    public LogHub(RedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task StreamLogs()
    {
        var logs = _redisService.GetLogs();

        foreach (var log in logs)
        {
            await Clients.Caller.SendAsync("ReceiveLog", log.Key, log.Value);
        }
    }
}