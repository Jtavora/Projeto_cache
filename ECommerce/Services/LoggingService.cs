using System.Text;
using System.Text.Json;
using E_Commerce.Interfaces;

namespace E_Commerce.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly HttpClient _httpClient;

        public LoggingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LogAsync(string message)
        {
            var logEntry = new
            {
                logId = Guid.NewGuid().ToString(),
                message = "[ECOMMERCE]" + message
            };

            var content = new StringContent(JsonSerializer.Serialize(logEntry), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://log-monitoring:5255/api/Logs", content);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Log enviado com sucesso.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao enviar log: {ex.Message}");
            }
        }
    }
}