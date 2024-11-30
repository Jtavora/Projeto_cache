using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LogMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly RedisService _redisService;

        public LogsController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost]
        public IActionResult AddLog([FromBody] LogEntry log)
        {
            if (string.IsNullOrEmpty(log?.Message))
            {
                return BadRequest("A mensagem do log não pode estar vazia.");
            }

            _redisService.AddLog(log.LogId, log.Message);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            var logs = _redisService.GetLogs();
            return Ok(logs);
        }
    }

    public class LogEntry
    {
        public string LogId { get; set; }
        public string Message { get; set; }
    }
}